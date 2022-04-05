namespace TarkovPriceViewer.Infrastructure.Entities
{
    public class Ballistic
    {
        public string Name;
        public string Damage;
        public string PP; // Penetration_power
        public string AD; // Armor_damage
        public string Accuracy;
        public string Recoil;
        public string FC; // Fragmentation_chance
        public string BL; // Bleed_L
        public string BH; // Bleed_H

        // Bullet Effectiveness Against Armor Class
        public string BE1;
        public string BE2;
        public string BE3;
        public string BE4;
        public string BE5;
        public string BE6;

        public string Special; // Subsonic or tracer

        public List<Ballistic> BallisticList;

        public Ballistic(
            string name, string damage, string pP, string aD,
            string accuracy, string recoil, string fC, string bL, string bH,
            string bE1, string bE2, string bE3, string bE4, string bE5, string bE6, string special, List<Ballistic> calibarlist)
        {
            Name = name;
            Damage = damage;
            PP = pP;
            AD = aD;
            Accuracy = accuracy;
            Recoil = recoil;
            FC = fC;
            BL = bL;
            BH = bH;
            BE1 = bE1;
            BE2 = bE2;
            BE3 = bE3;
            BE4 = bE4;
            BE5 = bE5;
            BE6 = bE6;
            Special = special;
            BallisticList = calibarlist;
        }

        public string[] Data()
        {
            return new string[] {
                Special
                , Name
                , Damage
                , BE1
                , BE2
                , BE3
                , BE4
                , BE5
                , BE6
            };
        }
    }
}
