use crate::shared::{CloudflareVariables, INTERNAL_SERVER_ERROR};
use serde::Serialize;
use time::macros::{format_description, offset};
use worker::*;

#[derive(Serialize)]
struct DataHeaders {
    result_code: u16,
}

#[derive(Serialize)]
struct DragaliaResponse<T> {
    data_headers: DataHeaders,
    data: T,
}

#[derive(Serialize)]
struct GetTextData {
    maintenance_text: String,
}

static RESULT_CODE_COMMON_MAINTENANCE: u16 = 101;
static RESULT_CODE_OK: u16 = 1;

pub fn handle_generic(
    _req: Request,
    _ctx: RouteContext<()>,
    _vars: CloudflareVariables,
) -> std::result::Result<Response, Error> {
    let resp = DragaliaResponse::<DataHeaders> {
        data_headers: DataHeaders {
            result_code: RESULT_CODE_COMMON_MAINTENANCE,
        },
        data: DataHeaders {
            result_code: RESULT_CODE_COMMON_MAINTENANCE,
        },
    };

    serialize(resp)
}

pub fn handle_get_text(
    _req: Request,
    _ctx: RouteContext<()>,
    vars: CloudflareVariables,
) -> Result<Response> {
    let xml = match generate_xml(vars) {
        Ok(xml) => xml,
        Err(err) => {
            console_error!("Failed to format XML response: {}", err);
            return Response::error(INTERNAL_SERVER_ERROR, 500);
        }
    };

    let resp = DragaliaResponse::<GetTextData> {
        data_headers: DataHeaders {
            result_code: RESULT_CODE_OK,
        },
        data: GetTextData {
            maintenance_text: xml,
        },
    };

    serialize(resp)
}

fn generate_xml(vars: CloudflareVariables) -> std::result::Result<String, &'static str> {
    // Convert to JST; the game expects all dates to be formatted without timezone and in JST
    let Ok(formatted_jst_date) = vars
        .end_date
        .to_offset(offset!(+9))
        .format(format_description!(
            "[year]-[month]-[day]T[hour]:[minute]:[second]"
        ))
    else {
        return Err("Failed to format parsed end date");
    };

    Ok(format!(
        "<title>{}</title>
<body>{}</body>
<schedule>Check back at:</schedule>
<date>{}</date>",
        vars.title, vars.body, formatted_jst_date
    ))
}

fn serialize<T: Serialize>(resp: T) -> Result<Response> {
    match rmp_serde::encode::to_vec_named(&resp) {
        Ok(bytes) => Response::from_bytes(bytes),
        Err(err) => {
            console_error!("Failed to serialize msgpack response: {}", err);
            return Response::error(INTERNAL_SERVER_ERROR, 500);
        }
    }
}
