using System;
using System.Text;
using Starcounter;

namespace JuicyTiles {
    public static class JuicyTilesSetupHandlers {
        public static void Setup() {
            JuicyTilesSetup.CreateIndex();

            Handle.POST("/launcher/juicytilessetup?{?}", (Request request, string key) => {
                Db.Transact(() => {
                    var setup = JuicyTilesSetup.GetSetup(key);
                    if (setup == null)
                        setup = new JuicyTilesSetup() { Key = key };
                    setup.Value = request.Body;
                });
                return 204;
            });

            Handle.GET("/launcher/juicytilessetup?{?}", (string key) => {
                var setup = JuicyTilesSetup.GetSetup(key);
                if (setup == null)
                    return 404;

                Response response = new Response();
                response.ContentType = "application/json";
                response.Body = setup.Value;
                response.StatusCode = 200;
                return response;
            });

            Handle.DELETE("/launcher/juicytilessetup?{?}", (string key) => {
                Db.Transact(() => {
                    if (key == "all") {
                        Db.SlowSQL("DELETE FROM JuicyTilesSetup");
                    } else {
                        var setup = JuicyTilesSetup.GetSetup(key);
                        if (setup != null) {
                            setup.Delete();
                        }
                    }
                });
                return 204;
            });

            Handle.GET("/launcher/generatestyles/{?}", (string app) => {
                string sql = "SELECT i FROM JuicyTiles.JuicyTilesSetup i WHERE i.Key LIKE ?";
                StringBuilder sb = new StringBuilder();
                int index = 0;

                app = "/" + app + "/%";
                sb.Append("INSERT INTO JuicyTiles.JuicyTilesSetup(\"Key\",\"Value\") VALUES").Append(Environment.NewLine);

                foreach (JuicyTiles.JuicyTilesSetup item in Db.SQL<JuicyTiles.JuicyTilesSetup>(sql, app)) {
                    if (index > 0) {
                        sb.Append(",").Append(Environment.NewLine);
                    }

                    sb.Append("    ('").Append(item.Key).Append("', '").Append(item.Value).Append("')");
                    index++;
                }

                return sb.ToString();
            });
        }
    }
}
