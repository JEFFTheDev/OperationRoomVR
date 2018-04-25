// Copyright (c) 2018 ManusVR
#if UNITY_EDITOR
using UnityEditor;


namespace Assets.ManusVR.Scripts.PhysicalInteraction
{
    public static class PhysicsPreferences
    {
        public const float SuggestedTimestep = 1 / 90f;

        public const float SuggestedGravityForce = -6f;

        public const byte SuggestedDefaultSolverIterations = 255;
        public const byte SuggestedDefaultSolverVelocityIterations = 255;

        public static bool ShouldPromptGravitySettings
        {
            get { return EditorPrefs.GetBool("PromptGravitySettings", true); }
            set { EditorPrefs.SetBool("PromptGravitySettings", value);}
        }

        public static bool ShouldPromptFixedTimestep
        {
            get { return EditorPrefs.GetBool("PromptFixedTimestep", true); }
            set { EditorPrefs.SetBool("PromptFixedTimestep", value); }
        }

        public static bool ShouldPromptDefaultSolverIterations
        {
            get { return EditorPrefs.GetBool("PromptDefaultSolverIterations", true); }
            set { EditorPrefs.SetBool("PromptDefaultSolverIterations ", value); }
        }

        public static bool ShouldPromptDefaultSolverVelocityIterations
        {
            get { return EditorPrefs.GetBool("PromptDefaultSolverVelocityIterations", true); }
            set { EditorPrefs.SetBool("PromptDefaultSolverVelocityIterations ", value); }
        }
    }

}
#endif
