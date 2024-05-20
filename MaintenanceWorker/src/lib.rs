pub mod game;
pub mod helper;
pub mod web;

use worker::*;

#[event(fetch)]
pub async fn main(req: Request, env: Env, _ctx: worker::Context) -> Result<Response> {
    let router = Router::new();

    router
        .post(
            "/2.19.0_:platform/maintenance/get_text",
            game::handle_get_text,
        )
        .post("/2.19.0_:platform/*path", game::handle_generic)
        .get("/", web::get_html)
        .get("/*path", web::get_html)
        .run(req, env)
        .await
}
