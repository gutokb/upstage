using Terraria.ModLoader;

namespace ExampleMod.Content.DamageClasses
{
    public class MoraleDamageClass : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
           
            if (damageClass == DamageClass.Generic)
                return StatInheritanceData.Full;
            return StatInheritanceData.None;
        }
    }
}