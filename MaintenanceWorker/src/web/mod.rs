use crate::shared::CloudflareVariables;
use time::{format_description::well_known::Iso8601, macros::format_description};
use worker::*;

static PAGE_HTML: &str = include_str!("page.html");

pub fn get_html(
    req: Request,
    _ctx: RouteContext<()>,
    vars: CloudflareVariables,
) -> Result<Response> {
    let formatted_html = match format_html_with_date(vars) {
        Ok(html) => html,
        Err(e) => {
            console_error!("Failed to parse HTML: {}", e);
            return Response::from_html(PAGE_HTML);
        }
    };

    return Response::from_html(formatted_html);
}

fn format_html_with_date(vars: CloudflareVariables) -> std::result::Result<String, &'static str> {
    let Ok(formatted_date) = vars.end_date.format(&Iso8601::DEFAULT) else {
        return Err("Failed to format parsed end date");
    };

    let final_html = PAGE_HTML
        .replace("{{ title }}", &vars.title)
        .replace("{{ body }}", &vars.body)
        .replace("{{ end_date }}", &formatted_date);

    return Ok(final_html);
}
