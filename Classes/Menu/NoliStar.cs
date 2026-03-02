using iiMenu.Mods;
using Photon.Realtime;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Video;
using JoinType = GorillaNetworking.JoinType;
using Random = UnityEngine.Random;
using Console = iiMenu.Classes.Menu.Console;
using iiMenu.Classes.Menu;

namespace Seravyn.Classes.Menu.Assets
{
    internal class NoliStar
    {
        private static int noliStarId = -1;
        private static float updatedTimeDelay;
        private static float respawnTime;
        private static Vector3 throwDirection;
        private static Vector3 networkedPosition;
        private static Quaternion networkedRotation;
        public static VRRig GhostRig;
        public static bool Coolo(VRRig Player) =>
            Player.isLocal || Player == GhostRig;



        private static NoliStarState noliStarState = NoliStarState.Default;
        private enum NoliStarState
        {
            Default,
            Throwing,
            Respawning
        }
        public static void Star()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                NoliStar1();
                NoliMusic();
            }
        }
        public static void NoStar()
        {
            {
                destroyNoliMusic();
                destroyNoliStar();

            }
        }
        public static void Star2()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                NoliStar2();
                NoliMusic2();
            }
        }
        public static void NoStar2()
        {
            {
                destroyNoliMusic2();
                destroyNoliStar2();

            }
        }
        public static void Star3()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                NoliStar3();
                NoliMusic3();
            }
        }
        public static void NoStar3()
        {
            {
                destroyNoliMusic3();
                destroyNoliStar3();

            }
        }
        public static void StarSIL()
        {
            if (ControllerInputPoller.instance.rightGrab)
            {
                NoliStarSIL();
            }
        }
        public static void NoStarSIL()
        {
            {
                destroyNoliStarSIL();

            }
        }
        public static void Star2SIL()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                NoliStar2SIL();
            }
        }
        public static void NoStar2SIL()
        {
            {
                destroyNoliStar2SIL();

            }
        }
        public static void Star3SIL()
        {
            if (ControllerInputPoller.instance.leftGrab)
            {
                NoliStar3SIL();
            }
        }
        public static void NoStar3SIL()
        {
            {
                destroyNoliStar3SIL();

            }
        }
        #region Right Hand Star
        public static void NoliStar1()
        {
            if (noliStarId < 0)
            {
                noliStarId = Console.GetFreeAssetID();

                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Star", noliStarId);
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "StarSpawn");
                Safety.FlushRPCs();
            }

            if (!Console.consoleAssets.ContainsKey(noliStarId))
                return;

            GameObject star = Console.consoleAssets[noliStarId].assetObject;

            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f && noliStarState == NoliStarState.Default)
            {
                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position, GorillaTagger.Instance.headCollider.transform.forward, out var RayPoint, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                GameObject Crosshair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Crosshair.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                Crosshair.transform.position = RayPoint.point == Vector3.zero ? RayPoint.transform.position + RayPoint.transform.forward * 20f : RayPoint.point;
                Crosshair.GetComponent<Renderer>().material.color = Color.white;

                UnityEngine.Object.Destroy(Crosshair, Time.deltaTime);
                UnityEngine.Object.Destroy(Crosshair.GetComponent<Collider>());
            }

            if (ControllerInputPoller.instance.rightControllerIndexFloat < 0.5f && holdingTrigger && noliStarState == NoliStarState.Default)
            {
                noliStarState = NoliStarState.Throwing;

                Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Throw");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "ThrowStar");

                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position, GorillaTagger.Instance.headCollider.transform.forward, out var RayPoint, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                throwDirection = (RayPoint.point - star.transform.position).normalized;
            }

            holdingTrigger = ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f;

            switch (noliStarState)
            {
                case NoliStarState.Default:
                    star.transform.position = GorillaTagger.Instance.rightHandTransform.position + Vector3.up * 0.2f;
                    star.transform.rotation = Quaternion.Euler(Time.time * 32f, Time.time * 10f, Time.time * 47f);
                    break;
                case NoliStarState.Throwing:
                    Physics.Raycast(star.transform.position, throwDirection, out var RayPoint, 0.5f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);

                    if (RayPoint.point == Vector3.zero)
                    {
                        star.transform.position += throwDirection * (Time.deltaTime * 15f);
                        star.transform.rotation = Quaternion.Euler(Time.time * 239f, Time.time * 201f, Time.time * 170f);
                    }
                    else
                    {
                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Explode");

                        bool kill = false;
                        foreach (VRRig rig in GorillaParent.instance.vrrigs)
                        {
                            if (PlayerIsLocal(rig))
                                continue;

                            if (Vector3.Distance(star.transform.position, rig.transform.position) < 2.32775f)
                            {
                                Console.ExecuteCommand("silkick", ReceiverGroup.All, RigManager.GetPlayerFromVRRig(rig).UserId);
                                kill = true;
                            }
                        }

                        Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", kill ? "KillStar" : "BreakStar");
                        noliStarState = NoliStarState.Respawning;
                        respawnTime = Time.time + 3f;
                    }

                    break;
                case NoliStarState.Respawning:
                    if (Time.time > respawnTime)
                    {
                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Default");
                        Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "StarSpawn");
                        noliStarState = NoliStarState.Default;
                    }

                    break;
            }

            if (Time.time > updatedTimeDelay && (networkedRotation != star.transform.rotation || networkedPosition != star.transform.position))
            {
                updatedTimeDelay = Time.time + 0.05f;

                networkedPosition = star.transform.position;
                networkedRotation = star.transform.rotation;

                Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, noliStarId, star.transform.position);
                Console.ExecuteCommand("asset-setrotation", ReceiverGroup.All, noliStarId, star.transform.rotation);
            }
        }
        public static bool PlayerIsLocal(VRRig Player) =>
           Player.isLocal || Player == GhostRig;

        public static void destroyNoliStar()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, noliStarId);
            noliStarId = -1;
            destroyNoliMusic();
            destroyNoliMusic2();
            destroyNoliMusic2SIL();
            destroyNoliMusic3();
            destroyNoliMusic3SIL();
            destroyNoliMusicSIL();

        }

        private static int noliMusicId = -1;
        private static bool holdingTrigger2;
        private static bool holdingTrigger;

        public static void NoliMusic()
        {
            if (noliMusicId < 0)
            {
                noliMusicId = Console.GetFreeAssetID();

                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "RangedMusic", noliMusicId);
                Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, noliMusicId, 0);

                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level1", "NoliLevel1");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level2", "NoliLevel2");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level3", "NoliLevel3");

                Safety.FlushRPCs();
            }
        }

        public static void destroyNoliMusic()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, noliMusicId);
            noliMusicId = -1;
        }
        #endregion
        #region Left Hand Star

        public static void NoliStar2()
        {
            if (noliStarId < 0)
            {
                noliStarId = Console.GetFreeAssetID();

                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Star", noliStarId);
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "StarSpawn");
                Safety.FlushRPCs();
            }

            if (!Console.consoleAssets.ContainsKey(noliStarId))
                return;

            GameObject star = Console.consoleAssets[noliStarId].assetObject;

            if (ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f && noliStarState == NoliStarState.Default)
            {
                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position, GorillaTagger.Instance.headCollider.transform.forward, out var RayPoint, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                GameObject Crosshair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Crosshair.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                Crosshair.transform.position = RayPoint.point == Vector3.zero ? RayPoint.transform.position + RayPoint.transform.forward * 20f : RayPoint.point;
                Crosshair.GetComponent<Renderer>().material.color = Color.white;

                UnityEngine.Object.Destroy(Crosshair, Time.deltaTime);
                UnityEngine.Object.Destroy(Crosshair.GetComponent<Collider>());
            }

            if (ControllerInputPoller.instance.leftControllerIndexFloat < 0.5f && holdingTrigger2 && noliStarState == NoliStarState.Default)
            {
                noliStarState = NoliStarState.Throwing;

                Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Throw");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "ThrowStar");

                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position, GorillaTagger.Instance.headCollider.transform.forward, out var RayPoint, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                throwDirection = (RayPoint.point - star.transform.position).normalized;
            }

            holdingTrigger2 = ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f;

            switch (noliStarState)
            {
                case NoliStarState.Default:
                    star.transform.position = GorillaTagger.Instance.leftHandTransform.position + Vector3.up * 0.2f;
                    star.transform.rotation = Quaternion.Euler(Time.time * 32f, Time.time * 10f, Time.time * 47f);
                    break;
                case NoliStarState.Throwing:
                    Physics.Raycast(star.transform.position, throwDirection, out var RayPoint, 0.5f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);

                    if (RayPoint.point == Vector3.zero)
                    {
                        star.transform.position += throwDirection * (Time.deltaTime * 15f);
                        star.transform.rotation = Quaternion.Euler(Time.time * 239f, Time.time * 201f, Time.time * 170f);
                    }
                    else
                    {
                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Explode");

                        bool kill = false;
                        foreach (VRRig rig in GorillaParent.instance.vrrigs)
                        {
                            if (PlayerIsLocal(rig))
                                continue;

                            if (Vector3.Distance(star.transform.position, rig.transform.position) < 2.32775f)
                            {
                                Console.ExecuteCommand("silkick", ReceiverGroup.All, RigManager.GetPlayerFromVRRig(rig).UserId);
                                kill = true;
                            }
                        }

                        Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", kill ? "KillStar" : "BreakStar");
                        noliStarState = NoliStarState.Respawning;
                        respawnTime = Time.time + 3f;
                    }

                    break;
                case NoliStarState.Respawning:
                    if (Time.time > respawnTime)
                    {
                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Default");
                        Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "StarSpawn");
                        noliStarState = NoliStarState.Default;
                    }

                    break;
            }

            if (Time.time > updatedTimeDelay && (networkedRotation != star.transform.rotation || networkedPosition != star.transform.position))
            {
                updatedTimeDelay = Time.time + 0.05f;

                networkedPosition = star.transform.position;
                networkedRotation = star.transform.rotation;

                Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, noliStarId, star.transform.position);
                Console.ExecuteCommand("asset-setrotation", ReceiverGroup.All, noliStarId, star.transform.rotation);
            }
        }
        public static void destroyNoliStar2()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, noliStarId);
            noliStarId = -1;
        }

        public static void NoliMusic2()
        {
            if (noliMusicId < 0)
            {
                noliMusicId = Console.GetFreeAssetID();

                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "RangedMusic", noliMusicId);
                Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, noliMusicId, 0);

                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level1", "NoliLevel1");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level2", "NoliLevel2");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level3", "NoliLevel3");

                Safety.FlushRPCs();
            }
        }

        public static void destroyNoliMusic2()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, noliMusicId);
            noliMusicId = -1;
        }
        #endregion
        #region Head Star
        public static void NoliStar3()
        {
            if (noliStarId < 0)
            {
                noliStarId = Console.GetFreeAssetID();

                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Star", noliStarId);
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "StarSpawn");
                Safety.FlushRPCs();
            }

            if (!Console.consoleAssets.ContainsKey(noliStarId))
                return;

            GameObject star = Console.consoleAssets[noliStarId].assetObject;

            if (ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f && noliStarState == NoliStarState.Default)
            {
                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position, GorillaTagger.Instance.headCollider.transform.forward, out var RayPoint, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                GameObject Crosshair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Crosshair.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                Crosshair.transform.position = RayPoint.point == Vector3.zero ? RayPoint.transform.position + RayPoint.transform.forward * 20f : RayPoint.point;
                Crosshair.GetComponent<Renderer>().material.color = Color.white;

                UnityEngine.Object.Destroy(Crosshair, Time.deltaTime);
                UnityEngine.Object.Destroy(Crosshair.GetComponent<Collider>());
            }

            if (ControllerInputPoller.instance.leftControllerIndexFloat < 0.5f && holdingTrigger2 && noliStarState == NoliStarState.Default)
            {
                noliStarState = NoliStarState.Throwing;

                Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Throw");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "ThrowStar");

                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position, GorillaTagger.Instance.headCollider.transform.forward, out var RayPoint, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                throwDirection = (RayPoint.point - star.transform.position).normalized;
            }

            holdingTrigger2 = ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f;

            switch (noliStarState)
            {
                case NoliStarState.Default:
                    star.transform.position = GorillaTagger.Instance.headCollider.transform.position + Vector3.up * 0.3f;
                    star.transform.rotation = Quaternion.Euler(Time.time * 32f, Time.time * 10f, Time.time * 47f);
                    break;
                case NoliStarState.Throwing:
                    Physics.Raycast(star.transform.position, throwDirection, out var RayPoint, 0.5f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);

                    if (RayPoint.point == Vector3.zero)
                    {
                        star.transform.position += throwDirection * (Time.deltaTime * 15f);
                        star.transform.rotation = Quaternion.Euler(Time.time * 239f, Time.time * 201f, Time.time * 170f);
                    }
                    else
                    {
                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Explode");

                        bool kill = false;
                        foreach (VRRig rig in GorillaParent.instance.vrrigs)
                        {
                            if (PlayerIsLocal(rig))
                                continue;

                            if (Vector3.Distance(star.transform.position, rig.transform.position) < 2.32775f)
                            {
                                Console.ExecuteCommand("silkick", ReceiverGroup.All, RigManager.GetPlayerFromVRRig(rig).UserId);
                                kill = true;
                            }
                        }

                        Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", kill ? "KillStar" : "BreakStar");
                        noliStarState = NoliStarState.Respawning;
                        respawnTime = Time.time + 3f;
                    }

                    break;
                case NoliStarState.Respawning:
                    if (Time.time > respawnTime)
                    {
                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Default");
                        Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "StarSpawn");
                        noliStarState = NoliStarState.Default;
                    }

                    break;
            }

            if (Time.time > updatedTimeDelay && (networkedRotation != star.transform.rotation || networkedPosition != star.transform.position))
            {
                updatedTimeDelay = Time.time + 0.05f;

                networkedPosition = star.transform.position;
                networkedRotation = star.transform.rotation;

                Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, noliStarId, star.transform.position);
                Console.ExecuteCommand("asset-setrotation", ReceiverGroup.All, noliStarId, star.transform.rotation);
            }
        }
        public static void destroyNoliStar3()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, noliStarId);
            noliStarId = -1;
        }

        public static void NoliMusic3()
        {
            if (noliMusicId < 0)
            {
                noliMusicId = Console.GetFreeAssetID();

                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "RangedMusic", noliMusicId);
                Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, noliMusicId, 0);

                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level1", "NoliLevel1");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level2", "NoliLevel2");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level3", "NoliLevel3");

                Safety.FlushRPCs();
            }
        }

        public static void destroyNoliMusic3()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, noliMusicId);
            noliMusicId = -1;
        }
        #endregion

        #region Right Hand Star SIL
        public static void NoliStarSIL()
        {
            if (noliStarId < 0)
            {
                noliStarId = Console.GetFreeAssetID();

                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Star", noliStarId);
                //Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "StarSpawn");
                Safety.FlushRPCs();
            }

            if (!Console.consoleAssets.ContainsKey(noliStarId))
                return;

            GameObject star = Console.consoleAssets[noliStarId].assetObject;

            if (ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f && noliStarState == NoliStarState.Default)
            {
                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position, GorillaTagger.Instance.headCollider.transform.forward, out var RayPoint, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                GameObject Crosshair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Crosshair.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                Crosshair.transform.position = RayPoint.point == Vector3.zero ? RayPoint.transform.position + RayPoint.transform.forward * 20f : RayPoint.point;
                Crosshair.GetComponent<Renderer>().material.color = Color.white;

                UnityEngine.Object.Destroy(Crosshair, Time.deltaTime);
                UnityEngine.Object.Destroy(Crosshair.GetComponent<Collider>());
            }

            if (ControllerInputPoller.instance.rightControllerIndexFloat < 0.5f && holdingTrigger && noliStarState == NoliStarState.Default)
            {
                noliStarState = NoliStarState.Throwing;

                Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Throw");
                //Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "ThrowStar");

                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position, GorillaTagger.Instance.headCollider.transform.forward, out var RayPoint, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                throwDirection = (RayPoint.point - star.transform.position).normalized;
            }

            holdingTrigger = ControllerInputPoller.instance.rightControllerIndexFloat > 0.5f;

            switch (noliStarState)
            {
                case NoliStarState.Default:
                    star.transform.position = GorillaTagger.Instance.rightHandTransform.position + Vector3.up * 0.2f;
                    star.transform.rotation = Quaternion.Euler(Time.time * 32f, Time.time * 10f, Time.time * 47f);
                    break;
                case NoliStarState.Throwing:
                    Physics.Raycast(star.transform.position, throwDirection, out var RayPoint, 0.5f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);

                    if (RayPoint.point == Vector3.zero)
                    {
                        star.transform.position += throwDirection * (Time.deltaTime * 15f);
                        star.transform.rotation = Quaternion.Euler(Time.time * 239f, Time.time * 201f, Time.time * 170f);
                    }
                    else
                    {
                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Explode");

                        bool kill = false;
                        foreach (VRRig rig in GorillaParent.instance.vrrigs)
                        {
                            if (PlayerIsLocal(rig))
                                continue;

                            if (Vector3.Distance(star.transform.position, rig.transform.position) < 2.32775f)
                            {
                                Console.ExecuteCommand("silkick", ReceiverGroup.All, RigManager.GetPlayerFromVRRig(rig).UserId);
                                kill = true;
                            }
                        }

                        //Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", kill ? "KillStar" : "BreakStar");
                        noliStarState = NoliStarState.Respawning;
                        respawnTime = Time.time + 3f;
                    }

                    break;
                case NoliStarState.Respawning:
                    if (Time.time > respawnTime)
                    {
                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Default");
                        //Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "StarSpawn");
                        noliStarState = NoliStarState.Default;
                    }

                    break;
            }

            if (Time.time > updatedTimeDelay && (networkedRotation != star.transform.rotation || networkedPosition != star.transform.position))
            {
                updatedTimeDelay = Time.time + 0.05f;

                networkedPosition = star.transform.position;
                networkedRotation = star.transform.rotation;

                Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, noliStarId, star.transform.position);
                Console.ExecuteCommand("asset-setrotation", ReceiverGroup.All, noliStarId, star.transform.rotation);
            }
        }
        public static void destroyNoliStarSIL()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, noliStarId);
            noliStarId = -1;
        }

        public static void NoliMusicSIL()
        {
            if (noliMusicId < 0)
            {
                noliMusicId = Console.GetFreeAssetID();

                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "RangedMusic", noliMusicId);
                Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, noliMusicId, 0);

                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level1", "NoliLevel1");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level2", "NoliLevel2");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level3", "NoliLevel3");

                Safety.FlushRPCs();
            }
        }

        public static void destroyNoliMusicSIL()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, noliMusicId);
            noliMusicId = -1;
        }
        #endregion
        #region Left Hand Star SIL

        public static void NoliStar2SIL()
        {
            if (noliStarId < 0)
            {
                noliStarId = Console.GetFreeAssetID();

                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Star", noliStarId);
                //Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "StarSpawn");
                Safety.FlushRPCs();
            }

            if (!Console.consoleAssets.ContainsKey(noliStarId))
                return;

            GameObject star = Console.consoleAssets[noliStarId].assetObject;

            if (ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f && noliStarState == NoliStarState.Default)
            {
                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position, GorillaTagger.Instance.headCollider.transform.forward, out var RayPoint, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                GameObject Crosshair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Crosshair.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                Crosshair.transform.position = RayPoint.point == Vector3.zero ? RayPoint.transform.position + RayPoint.transform.forward * 20f : RayPoint.point;
                Crosshair.GetComponent<Renderer>().material.color = Color.white;

                UnityEngine.Object.Destroy(Crosshair, Time.deltaTime);
                UnityEngine.Object.Destroy(Crosshair.GetComponent<Collider>());
            }

            if (ControllerInputPoller.instance.leftControllerIndexFloat < 0.5f && holdingTrigger2 && noliStarState == NoliStarState.Default)
            {
                noliStarState = NoliStarState.Throwing;

                Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Throw");
                //Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "ThrowStar");

                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position, GorillaTagger.Instance.headCollider.transform.forward, out var RayPoint, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                throwDirection = (RayPoint.point - star.transform.position).normalized;
            }

            holdingTrigger2 = ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f;

            switch (noliStarState)
            {
                case NoliStarState.Default:
                    star.transform.position = GorillaTagger.Instance.leftHandTransform.position + Vector3.up * 0.2f;
                    star.transform.rotation = Quaternion.Euler(Time.time * 32f, Time.time * 10f, Time.time * 47f);
                    break;
                case NoliStarState.Throwing:
                    Physics.Raycast(star.transform.position, throwDirection, out var RayPoint, 0.5f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);

                    if (RayPoint.point == Vector3.zero)
                    {
                        star.transform.position += throwDirection * (Time.deltaTime * 15f);
                        star.transform.rotation = Quaternion.Euler(Time.time * 239f, Time.time * 201f, Time.time * 170f);
                    }
                    else
                    {
                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Explode");

                        bool kill = false;
                        foreach (VRRig rig in GorillaParent.instance.vrrigs)
                        {
                            if (PlayerIsLocal(rig))
                                continue;

                            if (Vector3.Distance(star.transform.position, rig.transform.position) < 2.32775f)
                            {
                                Console.ExecuteCommand("silkick", ReceiverGroup.All, RigManager.GetPlayerFromVRRig(rig).UserId);
                                kill = true;
                            }
                        }

                        //Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", kill ? "KillStar" : "BreakStar");
                        noliStarState = NoliStarState.Respawning;
                        respawnTime = Time.time + 3f;
                    }

                    break;
                case NoliStarState.Respawning:
                    if (Time.time > respawnTime)
                    {
                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Default");
                        //Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "StarSpawn");
                        noliStarState = NoliStarState.Default;
                    }

                    break;
            }

            if (Time.time > updatedTimeDelay && (networkedRotation != star.transform.rotation || networkedPosition != star.transform.position))
            {
                updatedTimeDelay = Time.time + 0.05f;

                networkedPosition = star.transform.position;
                networkedRotation = star.transform.rotation;

                Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, noliStarId, star.transform.position);
                Console.ExecuteCommand("asset-setrotation", ReceiverGroup.All, noliStarId, star.transform.rotation);
            }
        }
        public static void destroyNoliStar2SIL()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, noliStarId);
            noliStarId = -1;
        }

        public static void NoliMusic2SIL()
        {
            if (noliMusicId < 0)
            {
                noliMusicId = Console.GetFreeAssetID();

                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "RangedMusic", noliMusicId);
                Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, noliMusicId, 0);

                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level1", "NoliLevel1");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level2", "NoliLevel2");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level3", "NoliLevel3");

                Safety.FlushRPCs();
            }
        }

        public static void destroyNoliMusic2SIL()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, noliMusicId);
            noliMusicId = -1;
        }
        #endregion
        #region Head Star SIL
        public static void NoliStar3SIL()
        {
            if (noliStarId < 0)
            {
                noliStarId = Console.GetFreeAssetID();

                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "Star", noliStarId);
                //Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "StarSpawn");
                Safety.FlushRPCs();
            }

            if (!Console.consoleAssets.ContainsKey(noliStarId))
                return;

            GameObject star = Console.consoleAssets[noliStarId].assetObject;

            if (ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f && noliStarState == NoliStarState.Default)
            {
                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position, GorillaTagger.Instance.headCollider.transform.forward, out var RayPoint, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                GameObject Crosshair = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Crosshair.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                Crosshair.transform.position = RayPoint.point == Vector3.zero ? RayPoint.transform.position + RayPoint.transform.forward * 20f : RayPoint.point;
                Crosshair.GetComponent<Renderer>().material.color = Color.white;

                UnityEngine.Object.Destroy(Crosshair, Time.deltaTime);
                UnityEngine.Object.Destroy(Crosshair.GetComponent<Collider>());
            }

            if (ControllerInputPoller.instance.leftControllerIndexFloat < 0.5f && holdingTrigger2 && noliStarState == NoliStarState.Default)
            {
                noliStarState = NoliStarState.Throwing;

                Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Throw");
                //Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "ThrowStar");

                Physics.Raycast(GorillaTagger.Instance.headCollider.transform.position, GorillaTagger.Instance.headCollider.transform.forward, out var RayPoint, 512f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);
                throwDirection = (RayPoint.point - star.transform.position).normalized;
            }

            holdingTrigger2 = ControllerInputPoller.instance.leftControllerIndexFloat > 0.5f;

            switch (noliStarState)
            {
                case NoliStarState.Default:
                    star.transform.position = GorillaTagger.Instance.headCollider.transform.position + Vector3.up * 0.4f;
                    star.transform.rotation = Quaternion.Euler(Time.time * 32f, Time.time * 10f, Time.time * 47f);
                    break;
                case NoliStarState.Throwing:
                    Physics.Raycast(star.transform.position, throwDirection, out var RayPoint, 0.5f, GorillaLocomotion.GTPlayer.Instance.locomotionEnabledLayers);

                    if (RayPoint.point == Vector3.zero)
                    {
                        star.transform.position += throwDirection * (Time.deltaTime * 15f);
                        star.transform.rotation = Quaternion.Euler(Time.time * 239f, Time.time * 201f, Time.time * 170f);
                    }
                    else
                    {
                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Explode");

                        bool kill = false;
                        foreach (VRRig rig in GorillaParent.instance.vrrigs)
                        {
                            if (PlayerIsLocal(rig))
                                continue;

                            if (Vector3.Distance(star.transform.position, rig.transform.position) < 2.32775f)
                            {
                                Console.ExecuteCommand("silkick", ReceiverGroup.All, RigManager.GetPlayerFromVRRig(rig).UserId);
                                kill = true;
                            }
                        }

                        // Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", kill ? "KillStar" : "BreakStar");
                        noliStarState = NoliStarState.Respawning;
                        respawnTime = Time.time + 3f;
                    }

                    break;
                case NoliStarState.Respawning:
                    if (Time.time > respawnTime)
                    {
                        Console.ExecuteCommand("asset-playanimation", ReceiverGroup.All, noliStarId, "Model", "Default");
                        //Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliStarId, "Model", "StarSpawn");
                        noliStarState = NoliStarState.Default;
                    }

                    break;
            }

            if (Time.time > updatedTimeDelay && (networkedRotation != star.transform.rotation || networkedPosition != star.transform.position))
            {
                updatedTimeDelay = Time.time + 0.05f;

                networkedPosition = star.transform.position;
                networkedRotation = star.transform.rotation;

                Console.ExecuteCommand("asset-setposition", ReceiverGroup.All, noliStarId, star.transform.position);
                Console.ExecuteCommand("asset-setrotation", ReceiverGroup.All, noliStarId, star.transform.rotation);
            }
        }
        public static void destroyNoliStar3SIL()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, noliStarId);
            noliStarId = -1;
        }

        public static void NoliMusic3SIL()
        {
            if (noliMusicId < 0)
            {
                noliMusicId = Console.GetFreeAssetID();

                Console.ExecuteCommand("asset-spawn", ReceiverGroup.All, "console.main1", "RangedMusic", noliMusicId);
                Console.ExecuteCommand("asset-setanchor", ReceiverGroup.All, noliMusicId, 0);

                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level1", "NoliLevel1");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level2", "NoliLevel2");
                Console.ExecuteCommand("asset-playsound", ReceiverGroup.All, noliMusicId, "Level3", "NoliLevel3");

                Safety.FlushRPCs();
            }
        }

        public static void destroyNoliMusic3SIL()
        {
            Console.ExecuteCommand("asset-destroy", ReceiverGroup.All, noliMusicId);
            noliMusicId = -1;
        }
        #endregion
    }
}
