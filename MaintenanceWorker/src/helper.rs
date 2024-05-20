pub mod vars {
    pub static TITLE: &'static str = "MAINTENANCE_TITLE";
    pub static BODY: &'static str = "MAINTENANCE_BODY";
    pub static END_DATE: &'static str = "MAINTENANCE_END_DATE";
}

pub mod util {
    use time::{format_description::well_known::Iso8601, OffsetDateTime};
    use worker::RouteContext;

    use super::vars;

    pub fn load_end_date(
        ctx: &RouteContext<()>,
    ) -> std::result::Result<OffsetDateTime, &'static str> {
        let Ok(end_date_var) = ctx.var(vars::END_DATE) else {
            return Err("Failed to load end date variable");
        };

        let Ok(parsed_date) = OffsetDateTime::parse(&end_date_var.to_string(), &Iso8601::DEFAULT)
        else {
            return Err("Failed to parse end date into date");
        };

        Ok(parsed_date)
    }
}
