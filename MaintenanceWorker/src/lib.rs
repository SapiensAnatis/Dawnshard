pub mod game;
pub mod shared;
pub mod web;

use shared::load_variables;
use worker::*;

macro_rules! with_variables {
    ( $x:expr ) => {{
        |req, ctx| match load_variables(&ctx) {
            Ok(vars) => return $x(req, ctx, vars),
            Err(err) => {
                console_error!("Error loading variables: {}", err);
                return Response::error("Internal Server Error", 500);
            }
        }
    }};
}

#[event(fetch)]
pub async fn main(req: Request, env: Env, _ctx: Context) -> worker::Result<Response> {
    let router = Router::new();

    router
        .post(
            "/2.19.0_:platform/maintenance/get_text",
            with_variables!(game::handle_get_text),
        )
        .post(
            "/2.19.0_:platform/*path",
            with_variables!(game::handle_generic),
        )
        .get("/", with_variables!(web::get_html))
        .get("/*path", with_variables!(web::get_html))
        .run(req, env)
        .await
}
