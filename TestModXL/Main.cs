using HarmonyLib;
using System.Reflection;
using UnityEngine;
using System.Collections;
using Cinemachine;
using UnityModManagerNet;

namespace TestModXL
{
#if DEBUG
    [EnableReloading]
#endif
    static class Main
    {
        static GameObject objToCreate;
        
        private static CinemachineFreeLook cinemachineFreeLook;
        public static bool isCameraPossessed = false;

        public static bool Enabled;
        private static Harmony Harmony;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            Settings.Instance = UnityModManager.ModSettings.Load<Settings>(modEntry);
            Settings.ModEntry = modEntry;

            modEntry.OnToggle = OnToggle;
#if DEBUG
            modEntry.OnUnload = Unload;
#endif
            modEntry.OnUpdate = OnUpdate;
            return true;
        }

        private static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                UITest();
            }
            throw new System.NotImplementedException();
        }

        private static void UITest()
        {
            if (!isCameraPossessed)
            {
                var actualCam = PlayerController.Instance.cameraController._actualCam;
                cameraControllerTraverse = Traverse.Create(PlayerController.Instance.cameraController).Field<bool>("_right");
                
                objToCreate = new GameObject("Camera Controller");
                Camera.main.gameObject.TryGetComponent<CinemachineBrain>(out var brain);
                if (brain == null)
                {
                    brain = Camera.main.gameObject.AddComponent<CinemachineBrain>();
                }

                brain.m_DefaultBlend.m_Time = 1;
                brain.m_ShowDebugText = true;

                cinemachineFreeLook = objToCreate.AddComponent<CinemachineFreeLook>();

                GameObject lookAt = new GameObject("LookAt");
                GameObject follow = new GameObject("Follow");
                
                lookAt.transform.position.Set(0,0,0);
                follow.transform.position.Set(1,1,1);
                
                cinemachineFreeLook.LookAt = lookAt.transform;
                cinemachineFreeLook.Follow = follow.transform;
                
                cinemachineFreeLook.ForceCameraPosition(lookAt.transform.position, Quaternion.identity);
                
                cinemachineFreeLook.Priority = 99;
                
                isCameraPossessed = true;
            }
        }


        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            if (Enabled == value) return true;
            Enabled = value;

            if (Enabled)
            {
                Harmony = new Harmony(modEntry.Info.Id);
                Harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            else
            {
                Harmony.UnpatchAll(Harmony.Id);
            }

            return true;
        }

#if DEBUG
        static bool Unload(UnityModManager.ModEntry modEntry)
        {
            Harmony?.UnpatchAll();
            return true;
        }
#endif
    }
}
