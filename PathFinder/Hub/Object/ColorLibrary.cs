using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RouteOptimizer.Object
{
    public class ColorLibrary
    {

        //public enum KnownColor : short
        //{
        //    ActiveBorder,
        //                 // system-defined color of // active window's border.

        //    ActiveCaption:2:
        //                  // system-defined color of // background of // active window's title bar.

        //    ActiveCaptionText:3:
        //                      // system-defined color of // text in // active window's title bar.

        //    AliceBlue:28:
        //              //system-defined color.

        //    AntiqueWhite:29:
        //                 //system-defined color.

        //    AppWorkspace:4:
        //                 // system-defined color of // application workspace. // application workspace is // are//in //multiple-document view that is not being occupied by documents.

        //    Aqua:30:
        //         //system-defined color.

        //    Aquamarine:31:
        //               //system-defined color.

        //    Azure:32:
        //          //system-defined color.

        //    Beige:33:
        //          //system-defined color.

        //    Bisque:34:
        //           //system-defined color.

        //    Black:35:
        //          //system-defined color.

        //    BlanchedAlmond:36:
        //                   //system-defined color.

        //    Blue:37:
        //         //system-defined color.

        //    BlueViolet:38:
        //               //system-defined color.

        //    Brown:39:
        //          //system-defined color.

        //    BurlyWood:40:
        //              //system-defined color.

        //    ButtonFace:168:
        //               // system-defined face color of //3-D element.

        //    ButtonHighlight:169:
        //                    // system-defined color that is // highlight color of //3-D element. This color is applied to parts of //3-D element that face // light source.

        //    ButtonShadow:170:
        //                 // system-defined color that is // shadow color of //3-D element. This color is applied to parts of //3-D element that face away from // light source.

        //    CadetBlue:41:
        //              //system-defined color.

        //    Chartreuse:42:
        //               //system-defined color.

        //    Chocolate:43:
        //              //system-defined color.

        //    Control:5:
        //            // system-defined face color of //3-D element.

        //    ControlDark:6:
        //                // system-defined shadow color of //3-D element. // shadow color is applied to parts of //3-D element that face away from // light source.

        //    ControlDarkDark:7:
        //                    // system-defined color that is // dark shadow color of //3-D element. // dark shadow color is applied to // parts of //3-D element that are // darkest color.

        //    ControlLight:8:
        //                 // system-defined color that is // light color of //3-D element. // light color is applied to parts of //3-D element that face // light source.

        //    ControlLightLight:9:
        //                      // system-defined highlight color of //3-D element. // highlight color is applied to // parts of //3-D element that are // lightest color.

        //    ControlText:10:
        //                // system-defined color of text in //3-D element.

        //    Coral:44:
        //          //system-defined color.

        //    CornflowerBlue:45:
        //                   //system-defined color.

        //    Cornsilk:46:
        //             //system-defined color.

        //    Crimson:47:
        //            //system-defined color.

        //    Cyan:48:
        //         //system-defined color.

        //    DarkBlue:49:
        //             //system-defined color.

        //    DarkCyan:50:
        //             //system-defined color.

        //    DarkGoldenrod:51:
        //                  //system-defined color.

        //    DarkGray:52:
        //             //system-defined color.

        //    DarkGreen:53:
        //              //system-defined color.

        //    DarkKhaki:54:
        //              //system-defined color.

        //    DarkMagenta:55:
        //                //system-defined color.

        //    DarkOliveGreen:56:
        //                   //system-defined color.

        //    DarkOrange:57:
        //               //system-defined color.

        //    DarkOrchid:58:
        //               //system-defined color.

        //    DarkRed:59:
        //            //system-defined color.

        //    DarkSalmon:60:
        //               //system-defined color.

        //    DarkSeaGreen:61:
        //                 //system-defined color.

        //    DarkSlateBlue:62:
        //                  //system-defined color.

        //    DarkSlateGray:63:
        //                  //system-defined color.

        //    DarkTurquoise:64:
        //                  //system-defined color.

        //    DarkViolet:65:
        //               //system-defined color.

        //    DeepPink:66:
        //             //system-defined color.

        //    DeepSkyBlue:67:
        //                //system-defined color.

        //    Desktop:11:
        //            // system-defined color of // desktop.

        //    DimGray:68:
        //            //system-defined color.

        //    DodgerBlue:69:
        //               //system-defined color.

        //    Firebrick:70:
        //              //system-defined color.

        //    FloralWhite:71:
        //                //system-defined color.

        //    ForestGreen:72:
        //                //system-defined color.

        //    Fuchsia:73:
        //            //system-defined color.

        //    Gainsboro:74:
        //              //system-defined color.

        //    GhostWhite:75:
        //               //system-defined color.

        //    Gold:76:
        //         //system-defined color.

        //    Goldenrod:77:
        //              //system-defined color.

        //    GradientActiveCaption:171:
        //                          // system-defined color of // lightest color in // color gradient of an active window's title bar.

        //    GradientInactiveCaption:172:
        //                            // system-defined color of // lightest color in // color gradient of an inactive window's title bar.

        //    Gray:78:
        //         //system-defined color.

        //    GrayText:12:
        //             // system-defined color of dimmed text. Items in //list that are disabled are displayed in dimmed text.

        //    Green:79:
        //          //system-defined color.

        //    GreenYellow:80:
        //                //system-defined color.

        //    Highlight:13:
        //              // system-defined color of // background of selected items. This includes selected menu items as well as selected text.

        //    HighlightText:14:
        //                  // system-defined color of // text of selected items.

        //    Honeydew:81:
        //             //system-defined color.

        //    HotPink:82:
        //            //system-defined color.

        //    HotTrack:15:
        //             // system-defined color used to designate //hot-tracked item. Single-clicking //hot-tracked item executes // item.

        //    InactiveBorder:16:
        //                   // system-defined color of an inactive window's border.

        //    InactiveCaption:17:
        //                    // system-defined color of // background of an inactive window's title bar.

        //    InactiveCaptionText:18:
        //                        // system-defined color of // text in an inactive window's title bar.

        //    IndianRed:83:
        //              //system-defined color.

        //    Indigo:84:
        //           //system-defined color.

        //    Info:19:
        //         // system-defined color of // background of //ToolTip.

        //    InfoText:20:
        //             // system-defined color of // text of //ToolTip.

        //    Ivory:85:
        //          //system-defined color.

        //    Khaki:86:
        //          //system-defined color.

        //    Lavender:87:
        //             //system-defined color.

        //    LavenderBlush:88:
        //                  //system-defined color.

        //    LawnGreen:89:
        //              //system-defined color.

        //    LemonChiffon:90:
        //                 //system-defined color.

        //    LightBlue:91:
        //              //system-defined color.

        //    LightCoral:92:
        //               //system-defined color.

        //    LightCyan:93:
        //              //system-defined color.

        //    LightGoldenrodYellow:94:
        //                         //system-defined color.

        //    LightGray:95:
        //              //system-defined color.

        //    LightGreen:96:
        //               //system-defined color.

        //    LightPink:97:
        //              //system-defined color.

        //    LightSalmon:98:
        //                //system-defined color.

        //    LightSeaGreen:99:
        //                  //system-defined color.

        //    LightSkyBlue:100:
        //                 //system-defined color.

        //    LightSlateGray:101:
        //                   //system-defined color.

        //    LightSteelBlue:102:
        //                   //system-defined color.

        //    LightYellow:103:
        //                //system-defined color.

        //    Lime:104:
        //         //system-defined color.

        //    LimeGreen:105:
        //              //system-defined color.

        //    Linen:106:
        //          //system-defined color.

        //    Magenta:107:
        //            //system-defined color.

        //    Maroon:108:
        //           //system-defined color.

        //    MediumAquamarine:109:
        //                     //system-defined color.

        //    MediumBlue:110:
        //               //system-defined color.

        //    MediumOrchid:111:
        //                 //system-defined color.

        //    MediumPurple:112:
        //                 //system-defined color.

        //    MediumSeaGreen:113:
        //                   //system-defined color.

        //    MediumSlateBlue:114:
        //                    //system-defined color.

        //    MediumSpringGreen:115:
        //                      //system-defined color.

        //    MediumTurquoise:116:
        //                    //system-defined color.

        //    MediumVioletRed:117:
        //                    //system-defined color.

        //    Menu:21:
        //         // system-defined color of //menu's background.

        //    MenuBar:173:
        //            // system-defined color of // background of //menu bar.

        //    MenuHighlight:174:
        //                  // system-defined color used to highlight menu items when // menu appears as //flat menu.

        //    MenuText:22:
        //             // system-defined color of //menu's text.

        //    MidnightBlue:118:
        //                 //system-defined color.

        //    MintCream:119:
        //              //system-defined color.

        //    MistyRose:120:
        //              //system-defined color.

        //    Moccasin:121:
        //             //system-defined color.

        //    NavajoWhite:122:
        //                //system-defined color.

        //    Navy:123:
        //         //system-defined color.

        //    OldLace:124:
        //            //system-defined color.

        //    Olive:125:
        //          //system-defined color.

        //    OliveDrab:126:
        //              //system-defined color.

        //    Orange:127:
        //           //system-defined color.

        //    OrangeRed:128:
        //              //system-defined color.

        //    Orchid:129:
        //           //system-defined color.

        //    PaleGoldenrod:130:
        //                  //system-defined color.

        //    PaleGreen:131:
        //              //system-defined color.

        //    PaleTurquoise:132:
        //                  //system-defined color.

        //    PaleVioletRed:133:
        //                  //system-defined color.

        //    PapayaWhip:134:
        //               //system-defined color.

        //    PeachPuff:135:
        //              //system-defined color.

        //    Peru:136:
        //         //system-defined color.

        //    Pink:137:
        //         //system-defined color.

        //    Plum:138:
        //         //system-defined color.

        //    PowderBlue:139:
        //               //system-defined color.

        //    Purple:140:
        //           //system-defined color.

        //    RebeccaPurple:175:
        //                  //system-defined color representing // ARGB value #663399.

        //    Red:141:
        //        //system-defined color.

        //    RosyBrown:142:
        //              //system-defined color.

        //    RoyalBlue:143:
        //              //system-defined color.

        //    SaddleBrown:144:
        //                //system-defined color.

        //    Salmon:145:
        //           //system-defined color.

        //    SandyBrown:146:
        //               //system-defined color.

        //    ScrollBar:23:
        //              // system-defined color of // background of //scroll bar.

        //    SeaGreen:147:
        //             //system-defined color.

        //    SeaShell:148:
        //             //system-defined color.

        //    Sienna:149:
        //           //system-defined color.

        //    Silver:150:
        //           //system-defined color.

        //    SkyBlue:151:
        //            //system-defined color.

        //    SlateBlue:152:
        //              //system-defined color.

        //    SlateGray:153:
        //              //system-defined color.

        //    Snow:154:
        //         //system-defined color.

        //    SpringGreen:155:
        //                //system-defined color.

        //    SteelBlue:156:
        //              //system-defined color.

        //    Tan:157:
        //        //system-defined color.

        //    Teal:158:
        //         //system-defined color.

        //    Thistle:159:
        //            //system-defined color.

        //    Tomato:160:
        //           //system-defined color.

        //    Transparent:27:
        //                //system-defined color.

        //    Turquoise:161:
        //              //system-defined color.

        //    Violet:162:
        //           //system-defined color.

        //    Wheat:163:
        //          //system-defined color.

        //    White:164:
        //          //system-defined color.

        //    WhiteSmoke:165:
        //               //system-defined color.

        //    Window:24:
        //           // system-defined color of // background in // client are//of //window.

        //    WindowFrame:25:
        //                // system-defined color of //window frame.

        //    WindowText:26,
        //               // system-defined color of // text in // client are//of //window.

        //    Yellow : 166,
        //           //system-defined color.

        //    YellowGreen:167:
        //                //system-defined color.
        //}
    }
}
