﻿public enum MouseButton
{
    Button1 = 0,
    Left = 0,
    Button2 = 1,
    Right = 1,
    Button3 = 2,
    Middle = 2,
    Button4 = 3,
    Button5 = 4,
    Button6 = 5,
    Button7 = 6,
    Button8 = 7,
}

public enum InputState : byte
{
    Release,
    Press,
    Repeat
}

public enum KeyboardKey
{
    Unknown = -1, // 0xFFFFFFFF
    Space = 32, // 0x00000020
    Apostrophe = 39, // 0x00000027
    Comma = 44, // 0x0000002C
    Minus = 45, // 0x0000002D
    Period = 46, // 0x0000002E
    Slash = 47, // 0x0000002F
    Alpha0 = 48, // 0x00000030
    Alpha1 = 49, // 0x00000031
    Alpha2 = 50, // 0x00000032
    Alpha3 = 51, // 0x00000033
    Alpha4 = 52, // 0x00000034
    Alpha5 = 53, // 0x00000035
    Alpha6 = 54, // 0x00000036
    Alpha7 = 55, // 0x00000037
    Alpha8 = 56, // 0x00000038
    Alpha9 = 57, // 0x00000039
    SemiColon = 59, // 0x0000003B
    Equal = 61, // 0x0000003D
    A = 65, // 0x00000041
    B = 66, // 0x00000042
    C = 67, // 0x00000043
    D = 68, // 0x00000044
    E = 69, // 0x00000045
    F = 70, // 0x00000046
    G = 71, // 0x00000047
    H = 72, // 0x00000048
    I = 73, // 0x00000049
    J = 74, // 0x0000004A
    K = 75, // 0x0000004B
    L = 76, // 0x0000004C
    M = 77, // 0x0000004D
    N = 78, // 0x0000004E
    O = 79, // 0x0000004F
    P = 80, // 0x00000050
    Q = 81, // 0x00000051
    R = 82, // 0x00000052
    S = 83, // 0x00000053
    T = 84, // 0x00000054
    U = 85, // 0x00000055
    V = 86, // 0x00000056
    W = 87, // 0x00000057
    X = 88, // 0x00000058
    Y = 89, // 0x00000059
    Z = 90, // 0x0000005A
    LeftBracket = 91, // 0x0000005B
    Backslash = 92, // 0x0000005C
    RightBracket = 93, // 0x0000005D
    GraveAccent = 96, // 0x00000060
    World1 = 161, // 0x000000A1
    World2 = 162, // 0x000000A2
    Escape = 256, // 0x00000100
    Enter = 257, // 0x00000101
    Tab = 258, // 0x00000102
    Backspace = 259, // 0x00000103
    Insert = 260, // 0x00000104
    Delete = 261, // 0x00000105
    Right = 262, // 0x00000106
    Left = 263, // 0x00000107
    Down = 264, // 0x00000108
    Up = 265, // 0x00000109
    PageUp = 266, // 0x0000010A
    PageDown = 267, // 0x0000010B
    Home = 268, // 0x0000010C
    End = 269, // 0x0000010D
    CapsLock = 280, // 0x00000118
    ScrollLock = 281, // 0x00000119
    NumLock = 282, // 0x0000011A
    PrintScreen = 283, // 0x0000011B
    Pause = 284, // 0x0000011C
    F1 = 290, // 0x00000122
    F2 = 291, // 0x00000123
    F3 = 292, // 0x00000124
    F4 = 293, // 0x00000125
    F5 = 294, // 0x00000126
    F6 = 295, // 0x00000127
    F7 = 296, // 0x00000128
    F8 = 297, // 0x00000129
    F9 = 298, // 0x0000012A
    F10 = 299, // 0x0000012B
    F11 = 300, // 0x0000012C
    F12 = 301, // 0x0000012D
    F13 = 302, // 0x0000012E
    F14 = 303, // 0x0000012F
    F15 = 304, // 0x00000130
    F16 = 305, // 0x00000131
    F17 = 306, // 0x00000132
    F18 = 307, // 0x00000133
    F19 = 308, // 0x00000134
    F20 = 309, // 0x00000135
    F21 = 310, // 0x00000136
    F22 = 311, // 0x00000137
    F23 = 312, // 0x00000138
    F24 = 313, // 0x00000139
    F25 = 314, // 0x0000013A
    Numpad0 = 320, // 0x00000140
    Numpad1 = 321, // 0x00000141
    Numpad2 = 322, // 0x00000142
    Numpad3 = 323, // 0x00000143
    Numpad4 = 324, // 0x00000144
    Numpad5 = 325, // 0x00000145
    Numpad6 = 326, // 0x00000146
    Numpad7 = 327, // 0x00000147
    Numpad8 = 328, // 0x00000148
    Numpad9 = 329, // 0x00000149
    NumpadDecimal = 330, // 0x0000014A
    NumpadDivide = 331, // 0x0000014B
    NumpadMultiply = 332, // 0x0000014C
    NumpadSubtract = 333, // 0x0000014D
    NumpadAdd = 334, // 0x0000014E
    NumpadEnter = 335, // 0x0000014F
    NumpadEqual = 336, // 0x00000150
    LeftShift = 340, // 0x00000154
    LeftControl = 341, // 0x00000155
    LeftAlt = 342, // 0x00000156
    LeftSuper = 343, // 0x00000157
    RightShift = 344, // 0x00000158
    RightControl = 345, // 0x00000159
    RightAlt = 346, // 0x0000015A
    RightSuper = 347, // 0x0000015B
    Menu = 348, // 0x0000015C
}
