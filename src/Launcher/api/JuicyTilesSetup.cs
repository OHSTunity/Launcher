using Starcounter;

namespace JuicyTiles {
    [Database]
    public class JuicyTilesSetup {
        public string Key;
        public string Value;

        public static void CreateIndex() {
//            Db.Transaction(() => {
                if (Db.SQL("SELECT i FROM MaterializedIndex i WHERE i.Name = ?", "JuicyTilesSetupKeyIndex").First == null)
                    Starcounter.Db.SQL("CREATE UNIQUE INDEX JuicyTilesSetupKeyIndex ON JuicyTilesSetup (Key ASC)");
//            });
        }

        public static JuicyTilesSetup GetSetup(string key) {
            return Db.SQL<JuicyTilesSetup>("SELECT c FROM JuicyTilesSetup c WHERE c.Key = ?", key).First;
        }
    }
}
