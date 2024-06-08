use time::{format_description::well_known::Iso8601, OffsetDateTime};
use worker::RouteContext;

static TITLE: &'static str = "MAINTENANCE_TITLE";
static BODY: &'static str = "MAINTENANCE_BODY";
static END_DATE: &'static str = "MAINTENANCE_END_DATE";

pub static INTERNAL_SERVER_ERROR: &'static str = "Internal Server Error";

pub struct CloudflareVariables {
    pub title: String,
    pub body: String,
    pub end_date: OffsetDateTime,
}

pub fn load_variables(
    ctx: &RouteContext<()>,
) -> std::result::Result<CloudflareVariables, VariableError> {
    let title = get_single_variable(ctx, TITLE)?;
    let body = get_single_variable(ctx, BODY)?;
    let end_date_str = get_single_variable(ctx, END_DATE)?;

    let Ok(end_date) = OffsetDateTime::parse(&end_date_str, &Iso8601::DEFAULT) else {
        return Err(VariableError::DateParseError {
            name: END_DATE,
            value: end_date_str,
        });
    };

    return Ok(CloudflareVariables {
        title,
        body,
        end_date,
    });

    fn get_single_variable(
        ctx: &RouteContext<()>,
        name: &'static str,
    ) -> Result<String, VariableError> {
        let Ok(var) = ctx.var(name) else {
            return Err(VariableError::NotFound { name });
        };

        return Ok(var.to_string());
    }
}

pub enum VariableError {
    NotFound { name: &'static str },
    DateParseError { name: &'static str, value: String },
}

impl std::fmt::Display for VariableError {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        match self {
            VariableError::NotFound { name } => {
                write!(f, "Variable {} was not defined in wrangler.toml", name)
            }
            VariableError::DateParseError { name, value } => write!(
                f,
                "Failed to parse variable {}: '{}' is not a valid ISO 8601 date time offset",
                name, value
            ),
        }
    }
}
