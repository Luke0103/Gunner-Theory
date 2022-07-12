using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DefaultValues
{
    public static int MaxPlayers { get { return 3; } }

    public static int BaseBank { get { return 300; } }
    public static int BasePrize { get { return 100; } }

    public static int MinChance { get { return 10; } }
    public static int MaxChance { get { return 90; } }
    public static int MinChangeRate { get { return 2; } }
    public static int MaxChangeRate { get { return 5; } }
    
    public static int ThiefValue { get { return 80; } }

    public static int WithdrawPerTurn { get { return 10; } }
    public static int MaxRound { get { return 4; } }

    public static float TurnDuration { get { return 35f; } }

    public enum EventCode : byte
    {
        START_GAME = 0,
        INSTANTIATE = 1,
        NEXT_TURN = 2,
        ROUND_OVER = 3,
        GAME_OVER = 4,
        PLAYER_CHANCE_CHANGE = 10,
        PLAYER_PRIZE_CHANGE = 11,
        PLAYER_ATTACK = 12,
        PLAYER_CHANGED_STANCE = 13,
        PLAYER_KILLED = 21,
        PLAYER_BANKRUPT = 22,
        PLAYER_ESCAPED = 23,
        CARD_USED = 30,
        CARD_SUICIDE = 31,
        CARD_ESCAPE = 32,
        CARD_FUND = 33,
        CARD_BRIBE = 34,
        CARD_REVERSE = 35,
        CARD_TERROR = 36,
        CARD_SABOTAGE = 37,
        CARD_THIEF = 38,
    }

    public enum CardCode : int
    {
        EMPTY = 0,
        PROVOKE = 1,
        SUICIDE = 2,
        OILING = 3,
        ESCAPE = 4,
        FUND = 5,
        BRIBE = 6,
        REVERSE = 7,
        TERROR = 8,
        SABOTAGE = 9,
        THIEF = 10
    }
}
