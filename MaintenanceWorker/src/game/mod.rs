use crate::helper::{util::load_end_date, vars};
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
) -> std::result::Result<Response, Error> {
    let resp = DragaliaResponse::<DataHeaders> {
        data_headers: DataHeaders {
            result_code: RESULT_CODE_COMMON_MAINTENANCE,
        },
        data: DataHeaders {
            result_code: RESULT_CODE_COMMON_MAINTENANCE,
        },
    };

    let Ok(bytes) = rmp_serde::encode::to_vec_named(&resp) else {
        return Err("Failed to serialize response".into());
    };

    Response::from_bytes(bytes)
}

pub fn handle_get_text(
    _req: Request,
    ctx: RouteContext<()>,
) -> std::result::Result<Response, Error> {
    let xml = match generate_xml(ctx) {
        Ok(xml) => xml,
        Err(e) => {
            console_error!("Failed to format XML response: {}", e);
            return Err("Failed to format XML response".into());
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

    let Ok(bytes) = rmp_serde::encode::to_vec_named(&resp) else {
        return Err("Failed to serialize response".into());
    };

    Response::from_bytes(bytes)
}

fn generate_xml(ctx: RouteContext<()>) -> std::result::Result<String, &'static str> {
    let parsed_date = match load_end_date(&ctx) {
        Ok(date) => date,
        Err(e) => {
            console_error!("Failed to load parsed end date: {}", e);
            return Err("Failed to load parsed end date");
        }
    };

    // Convert to JST; the game expects all dates to be formatted without timezone and in JST
    let Ok(formatted_jst_date) = parsed_date
        .to_offset(offset!(+9))
        .format(format_description!(
            "[year]-[month]-[day]T[hour]:[minute]:[second]"
        ))
    else {
        return Err("Failed to format parsed end date");
    };

    let Ok(maintenance_title) = ctx.var(vars::TITLE) else {
        return Err("Failed to load maintenance title");
    };

    let Ok(maintenance_body) = ctx.var(vars::BODY) else {
        return Err("Failed to load maintenance body");
    };

    Ok(format!(
        "<title>{}</title>
<body>{}</body>
<schedule>Check back at:</schedule>
<date>{}</date>",
        maintenance_title.to_string(),
        maintenance_body.to_string(),
        formatted_jst_date
    ))
}
