use crate::helper::util::load_end_date;
use time::macros::format_description;
use worker::*;

static PAGE_HTML: &str = include_str!("page.html");

pub fn get_html(_req: Request, ctx: RouteContext<()>) -> std::result::Result<Response, Error> {
    let formatted_html = match format_html_with_date(ctx) {
        Ok(html) => html,
        Err(e) => {
            console_error!("Failed to parse HTML: {}", e);
            return Response::from_html(PAGE_HTML);
        }
    };

    return Response::from_html(formatted_html);
}

fn format_html_with_date(ctx: RouteContext<()>) -> std::result::Result<String, &'static str> {
    let parsed_date = match load_end_date(&ctx) {
        Ok(date) => date,
        Err(e) => {
            console_error!("Failed to load parsed end date: {}", e);
            return Err("Failed to load parsed end date");
        }
    };

    let Ok(formatted_date) = parsed_date.format(format_description!(
        "[day]/[month]/[year] [hour]:[minute]:[second] +[offset_hour]:[offset_minute]"
    )) else {
        return Err("Failed to format parsed end date");
    };

    let date_para = format!(
        "<p class=\"date\">Expected end date: {}</p>",
        formatted_date
    );

    return Ok(PAGE_HTML.replace("<p class=\"date\" hidden></p>", &date_para));
}
