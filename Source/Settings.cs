﻿using System.Text;
using UnityEngine;
using Verse;

namespace ImpassableMapMaker
{
    public class SettingsController : Mod
    {
        public SettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "Impassable Map Maker";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }

    public enum ImpassableShape
    {
        Square,
        Round
    }

    public class Settings : ModSettings
    {
        private const int GAP_SIZE = 20;
        private const float DEFAULT_PERCENT_OFFSET = 5f;
        private const int DEFAULT_OPEN_AREA_SIZE = 54;
        private const int DEFAULT_WALLS_SMOOTHNESS = 10;
        private const int DEFAULT_PEREMETER_BUFFER = 6;
        private const int DEFAULT_QUARY_SIZE = 5;
        private const float DEFAULT_MOVEMENT_DIFFICULTY = 4.5f;

        private static Vector2 scrollPosition = Vector2.zero;

        private static float percentOffset = DEFAULT_PERCENT_OFFSET;
        public static float PercentOffset { get { return percentOffset; } }
        public static ImpassableShape OpenAreaShape = ImpassableShape.Square;
        public static int OpenAreaSizeX = DEFAULT_OPEN_AREA_SIZE;
        public static int OpenAreaSizeZ = DEFAULT_OPEN_AREA_SIZE;
        public static int MiddleWallSmoothness = 10;
        public static int PeremeterBuffer = DEFAULT_PEREMETER_BUFFER;
        public static bool HasMiddleArea = true;
        public static ImpassableShape shape = ImpassableShape.Square;
        public static bool ScatteredRocks = true;
        public static bool IncludeQuarySpot = false;
        public static int QuarySize = DEFAULT_QUARY_SIZE;
        public static float MovementDifficulty = DEFAULT_MOVEMENT_DIFFICULTY;
        private static string movementDifficultyBuffer = DEFAULT_MOVEMENT_DIFFICULTY.ToString();

        public override void ExposeData()
        {
            base.ExposeData();

            string s = shape.ToString();
            string openAreaShape = OpenAreaShape.ToString();

            Scribe_Values.Look<bool>(ref HasMiddleArea, "ImpassableMapMaker.hasMiddleArea", true, false);
            Scribe_Values.Look<float>(ref percentOffset, "ImpassableMapMaker.percentOffset", DEFAULT_PERCENT_OFFSET, false);
            Scribe_Values.Look<string>(ref openAreaShape, "ImpassableMapMaker.OpenAreaShape", ImpassableShape.Square.ToString(), false);
            Scribe_Values.Look<int>(ref OpenAreaSizeX, "ImpassableMapMaker.OpenAreaSizeX", DEFAULT_OPEN_AREA_SIZE, false);
            Scribe_Values.Look<int>(ref OpenAreaSizeZ, "ImpassableMapMaker.OpenAreaSizeZ", DEFAULT_OPEN_AREA_SIZE, false);
            Scribe_Values.Look<int>(ref PeremeterBuffer, "ImpassableMapMaker.PeremeterBuffer", DEFAULT_PEREMETER_BUFFER, false);
            Scribe_Values.Look<int>(ref MiddleWallSmoothness, "ImpassableMapMaker.MakeWallsSmooth", DEFAULT_WALLS_SMOOTHNESS, false);
            Scribe_Values.Look<string>(ref s, "ImpassableMapMaker.Shape", ImpassableShape.Square.ToString(), false);
            Scribe_Values.Look<bool>(ref ScatteredRocks, "ImpassableMapMaker.scatteredRocks", true, false);
            Scribe_Values.Look<bool>(ref IncludeQuarySpot, "ImpassableMapMaker.IncludeQuarySpot", false, false);
            Scribe_Values.Look<int>(ref QuarySize, "ImpassableMapMaker.QuarySize", DEFAULT_QUARY_SIZE, false);
            Scribe_Values.Look<float>(ref MovementDifficulty, "ImpassableMapMaker.MovementDifficulty", DEFAULT_MOVEMENT_DIFFICULTY, false);
            movementDifficultyBuffer = MovementDifficulty.ToString();

            if (Scribe.mode != LoadSaveMode.Saving)
            {
                if (ImpassableShape.Round.ToString().Equals(s))
                {
                    shape = ImpassableShape.Round;
                }
                else
                {
                    shape = ImpassableShape.Square;
                }

                if (ImpassableShape.Round.ToString().Equals(openAreaShape))
                {
                    OpenAreaShape = ImpassableShape.Round;
                }
                else
                {
                    OpenAreaShape = ImpassableShape.Square;
                }
            }
        }

        public static void DoSettingsWindowContents(Rect rect)
        {
            Rect scroll = new Rect(5f, 45f, 430, rect.height);
            Rect view = new Rect(0, 45, 400, 800);

            Widgets.BeginScrollView(scroll, ref scrollPosition, view, true);
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(view);

            ls.TextFieldNumericLabeled<float>(
                "ImpassableMapMaker.WorldMapMovementDifficulty".Translate(), 
                ref MovementDifficulty, ref movementDifficultyBuffer, 1f, 100f);
            if (ls.ButtonText("ImpassableMapMaker.Default".Translate()))
            {
                MovementDifficulty = DEFAULT_MOVEMENT_DIFFICULTY;
                movementDifficultyBuffer = DEFAULT_MOVEMENT_DIFFICULTY.ToString();
            }
            ls.GapLine(GAP_SIZE);

            ls.Label("ImpassableMapMaker.MountainShape".Translate());
            if (ls.RadioButton("ImpassableMapMaker.ShapeSquare".Translate(), shape == ImpassableShape.Square))
            {
                shape = ImpassableShape.Square;
            }
            if (ls.RadioButton("ImpassableMapMaker.ShapeRound".Translate(), shape == ImpassableShape.Round))
            {
                shape = ImpassableShape.Round;
            }
            ls.Gap(GAP_SIZE);

            ls.CheckboxLabeled("ImpassableMapMaker.ScatteredRocks".Translate(), ref ScatteredRocks);
            ls.GapLine(GAP_SIZE);

            ls.CheckboxLabeled("ImpassableMapMaker.HasMiddleArea".Translate(), ref HasMiddleArea);
            ls.Gap(6);

            if (HasMiddleArea)
            {
                ls.Label("ImpassableMapMaker.MiddleAreaShape".Translate());
                ls.Gap(2);
                if (ls.RadioButton("ImpassableMapMaker.ShapeSquare".Translate(), OpenAreaShape == ImpassableShape.Square))
                {
                    OpenAreaShape = ImpassableShape.Square;
                }
                if (ls.RadioButton("ImpassableMapMaker.ShapeRound".Translate(), OpenAreaShape == ImpassableShape.Round))
                {
                    OpenAreaShape = ImpassableShape.Round;
                }
                ls.Gap(2);

                ls.Label("ImpassableMapMaker.OpenAreaSize".Translate());
                if (OpenAreaShape == ImpassableShape.Square)
                {
                    ls.Label("ImpassableMapMaker.Width".Translate() + ": " + OpenAreaSizeZ);
                    OpenAreaSizeZ = (int)ls.Slider(OpenAreaSizeZ, 40, 75);
                    ls.Label("ImpassableMapMaker.Height".Translate() + ": " + OpenAreaSizeX);
                    OpenAreaSizeX = (int)ls.Slider(OpenAreaSizeX, 40, 75);
                }
                else
                {
                    ls.Label("ImpassableMapMaker.Radius".Translate() + ": " + OpenAreaSizeZ);
                    OpenAreaSizeX = (int)ls.Slider(OpenAreaSizeX, 40, 75);
                    OpenAreaSizeZ = OpenAreaSizeX;
                }
                if (ls.ButtonText("ImpassableMapMaker.Default".Translate()))
                {
                    OpenAreaSizeX = DEFAULT_OPEN_AREA_SIZE;
                    OpenAreaSizeZ = DEFAULT_OPEN_AREA_SIZE;
                }
                ls.Gap(GAP_SIZE);

                ls.Label("ImpassableMapMaker.OpenAreaMaxOffsetFromMiddle".Translate() + ": " + percentOffset.ToString("N1") + "%");
                percentOffset = ls.Slider(percentOffset, 0, 25);
                if (ls.ButtonText("ImpassableMapMaker.Default".Translate()))
                {
                    percentOffset = DEFAULT_PERCENT_OFFSET;
                }
                ls.Gap(GAP_SIZE);

                ls.Label("ImpassableMapMaker.MiddleOpeningWallSmoothnes".Translate());
                ls.Label("<< " + "ImpassableMapMaker.Smooth".Translate() + " -- " + "ImpassableMapMaker.Rough".Translate() + " >>");
                MiddleWallSmoothness = (int)ls.Slider(MiddleWallSmoothness, 0, 20);
                if (ls.ButtonText("ImpassableMapMaker.Default".Translate()))
                {
                    MiddleWallSmoothness = 10;
                }
                ls.GapLine(GAP_SIZE);
            }

            if (shape == ImpassableShape.Square)
            {
                ls.Label("ImpassableMapMaker.EdgeBuffer".Translate() + ": " + PeremeterBuffer.ToString());
                ls.Label("<< " + "ImpassableMapMaker.Smaller".Translate() + " -- " + "ImpassableMapMaker.Larger".Translate() + " >>");
                PeremeterBuffer = (int)ls.Slider(PeremeterBuffer, 3, 30);
                if (ls.ButtonText("ImpassableMapMaker.Default".Translate()))
                {
                    PeremeterBuffer = DEFAULT_PEREMETER_BUFFER;
                }
                ls.Label("ImpassableMapMaker.EdgeBufferWarning".Translate());
                ls.GapLine(GAP_SIZE);
            }

            ls.CheckboxLabeled("ImpassableMapMaker.IncludeQuarySpot".Translate(), ref IncludeQuarySpot);
            if (IncludeQuarySpot)
            {
                ls.Label("<< " + "ImpassableMapMaker.Smaller".Translate() + " -- " + "ImpassableMapMaker.Larger".Translate() + " >>");
                QuarySize = (int)ls.Slider(QuarySize, 3, 20);
                StringBuilder sb = new StringBuilder("(");
                sb.Append(QuarySize.ToString());
                sb.Append(", ");
                sb.Append(QuarySize.ToString());
                sb.Append(")");
                ls.Label(sb.ToString());
                if (ls.ButtonText("ImpassableMapMaker.Default".Translate()))
                {
                    QuarySize = DEFAULT_QUARY_SIZE;
                }
            }
            
            ls.End();
            Widgets.EndScrollView();
        }
    }
}