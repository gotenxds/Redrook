// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem
{

    /// <summary>
    /// Utility methods for working with StandardDialogueUI panel numbers.
    /// </summary>
    public static class PanelNumberUtility
    {

        public static SubtitlePanelNumber IntToSubtitlePanelNumber(int i)
        {
            return (0 <= i && i <= 15) ? (SubtitlePanelNumber)i : SubtitlePanelNumber.Default;
        }

        public static int GetSubtitlePanelIndex(SubtitlePanelNumber subtitlePanelNumber)
        {
            switch (subtitlePanelNumber)
            {
                case SubtitlePanelNumber.Panel0: return 0;
                case SubtitlePanelNumber.Panel1: return 1;
                case SubtitlePanelNumber.Panel2: return 2;
                case SubtitlePanelNumber.Panel3: return 3;
                case SubtitlePanelNumber.Panel4: return 4;
                case SubtitlePanelNumber.Panel5: return 5;
                case SubtitlePanelNumber.Panel6: return 6;
                case SubtitlePanelNumber.Panel7: return 7;
                case SubtitlePanelNumber.Panel8: return 8;
                case SubtitlePanelNumber.Panel9: return 9;
                case SubtitlePanelNumber.Panel10: return 10;
                case SubtitlePanelNumber.Panel11: return 11;
                case SubtitlePanelNumber.Panel12: return 12;
                case SubtitlePanelNumber.Panel13: return 13;
                case SubtitlePanelNumber.Panel14: return 14;
                case SubtitlePanelNumber.Panel15: return 15;
                default: return -1;
            }
        }

        public static int GetMenuPanelIndex(MenuPanelNumber menuPanelNumber)
        {
            switch (menuPanelNumber)
            {
                case MenuPanelNumber.Panel0: return 0;
                case MenuPanelNumber.Panel1: return 1;
                case MenuPanelNumber.Panel2: return 2;
                case MenuPanelNumber.Panel3: return 3;
                case MenuPanelNumber.Panel4: return 4;
                case MenuPanelNumber.Panel5: return 5;
                case MenuPanelNumber.Panel6: return 6;
                case MenuPanelNumber.Panel7: return 7;
                case MenuPanelNumber.Panel8: return 8;
                case MenuPanelNumber.Panel9: return 9;
                case MenuPanelNumber.Panel10: return 10;
                case MenuPanelNumber.Panel11: return 11;
                case MenuPanelNumber.Panel12: return 12;
                case MenuPanelNumber.Panel13: return 13;
                case MenuPanelNumber.Panel14: return 14;
                case MenuPanelNumber.Panel15: return 15;
                default: return -1;
            }
        }

    }
}
