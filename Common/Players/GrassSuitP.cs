using Terraria.ModLoader;

namespace upstage.Common.Players
{
    public class GrassSuitP : ModPlayer
{
    public bool GrassSuitBonus = false;
    public override void PostUpdateRunSpeeds()
    {
        if (GrassSuitBonus)
        {
            Player.runAcceleration *= 3f;
        }
    }

    public override void ResetEffects()
    {
        GrassSuitBonus = false;
    }
}
}
