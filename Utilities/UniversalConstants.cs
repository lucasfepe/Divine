using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UniversalConstants 
{
    public const int LIGHT_WIN_THRESHOLD = 1000;
    public const float FIELD_CARD_SCALE = .8f;
    public const float HAND_CARD_SCALE = .65f;
    public const float INSPECT_CARD_SCALE = 3f;
    public const float CARD_PREVIEW_SCALE = 2.5f;
    public const float CARD_CATALOGUE_SCALE = 1f;
    public const float CARD_PILE_OFFSET = 1f;
    public const string ABSORB_DESC = "Select one enemy star of lesser rank. Absorb the target. This card gains stardust equal to the current stardust of the target. The target vanishes. Can be used once.";
    public const string BONUS_ENERGY_DESCRIPTION = "            Adds \"Inspired\" status to\ntarget. Status resolves at the beginning\nof the next turn to gain target's bonus\nenergy type by the POWER amount.";
    public const string GAIN_EXPERIENCE_DESCRIPTION = "              Adds \"Enlightened\" status\r\nto target. Status resolves at the\r\nbeginning of the next turn to gain\r\nexperience equal to POWER amount.";
    public const string ATTACK_DESCRIPTION = "         Decreases health of \ntarget by the POWER amount. If\ntarget's health drops to 0 target dies.";
    public const string EUREKA_DESCRIPTION = "          Enables you to play one\n\"Laboratory\" civilization card this turn.\nNote: You still have to pay its cost.";
    public const float YELLOW_R = 239f/255;
    public const float YELLOW_G = 208f / 255;
    public const float YELLOW_B = 29f / 255;
    public const float RED_R = 208f / 255;
    public const float RED_G = 58f / 255;
    public const float RED_B = 52f / 255;
    public const float GREEN_R = 39f / 255;
    public const float GREEN_G = 207f / 255;
    public const float GREEN_B = 104f / 255;
    public const float BLUE_R = 54f / 255;
    public const float BLUE_G = 152f / 255;
    public const float BLUE_B = 204f / 255;
    public const float ORANGE_R = 238f / 255;
    public const float ORANGE_G = 130f / 255;
    public const float ORANGE_B = 17f / 255;
    public const float WHITE_R = 255f / 255;
    public const float WHITE_G = 250f / 255;
    public const float WHITE_B = 245f / 255;

    public static Color GetYellow()
    {
        return new Color(YELLOW_R, YELLOW_G, YELLOW_B,1f);
    }
    public static Color GetRed()
    {
        return new Color(RED_R, RED_G, RED_B, 1f);
    }
    public static Color GetGreen()
    {
        return new Color(GREEN_R, GREEN_G, GREEN_B, 1f);
    }
    public static Color GetOrange()
    {
        return new Color(ORANGE_R, ORANGE_G, ORANGE_B, 1f);
    }
    public static Color GetWhite()
    {
        return new Color(WHITE_R, WHITE_G, WHITE_B, 1f);
    }

    public static Color GetBlue()
    {
        return new Color(BLUE_R, BLUE_G, BLUE_B, 1f);
    }
}
