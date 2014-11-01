using Starcounter;

namespace JuicyTiles {
    public static class JuicyTilesSetupHandlers {
        public static void Setup() {
            JuicyTilesSetup.CreateIndex();

            Handle.PUT("/juicytilessetup?{?}", (Request request, string key) => {
                Db.Transaction(() => {
                    var setup = JuicyTilesSetup.GetSetup(key);
                    if (setup == null)
                        setup = new JuicyTilesSetup() { Key = key };
                    setup.Value = request.Body;
                });
                return 204;
            });

            Handle.GET("/juicytilessetup?{?}", (string key) => {
                var setup = JuicyTilesSetup.GetSetup(key);
                if (setup == null)
                    return 404;

                Response response = new Response();
                response.ContentType = "application/json";
                response.Body = setup.Value;
                response.StatusCode = 200;
                return response;
            });

            Handle.DELETE("/juicytilessetup?{?}", (string key) => {
                var setup = JuicyTilesSetup.GetSetup(key);
                if (setup != null) {
                    Db.Transaction(() => {
                        setup.Delete();
                    });
                }
                return 204;
            });
        }
    }
}
