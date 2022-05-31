using UnityEngine;
using Verse;

namespace RimGoddess.Race;

public class Dialog_FaithPedestal : Window
{
    public override Vector2 InitialSize => new Vector2(400f, 200f);

    public override void DoWindowContents(Rect a_inRect)
    {
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
        Widgets.Label(new Rect(0f, 0f, a_inRect.width, a_inRect.height),
            "RTN_Translation_FaithPedestalWarning".Translate());
        if (Widgets.ButtonText(new Rect(0f, a_inRect.height - 35f, a_inRect.width * 0.5f, 35f), "Ok".Translate()))
        {
            Find.WindowStack.TryRemove(this);
        }
    }
}