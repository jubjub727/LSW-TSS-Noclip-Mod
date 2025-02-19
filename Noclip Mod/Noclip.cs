using OMP.LSWTSS.CApi1;
using System.Numerics;

namespace OMP.LSWTSS;

public class Noclip
{
    static DateTime lastFrameTime;
    static float X, Y, Z;
    static bool noclipEnabled = false;

    InputHook1.Client _inputHookClient;

    CFuncHook1<GameFramework.ProcessMethod.NativeDelegate> _gameFrameworkProcessMethodHook;

    public static bool InputHookClientHandler(in InputHook1.NativeMessage inputHookClientNativeMessage)
    {
        if (inputHookClientNativeMessage.WParam == Config.ToggleKey)
        {
            if ((PInvoke.User32.WindowMessage)inputHookClientNativeMessage.Type == PInvoke.User32.WindowMessage.WM_KEYUP)
            {
                noclipEnabled = !noclipEnabled;
                return true;
            }
        }

        if (inputHookClientNativeMessage.WParam == Config.ReloadKey)
        {
            if ((PInvoke.User32.WindowMessage)inputHookClientNativeMessage.Type == PInvoke.User32.WindowMessage.WM_KEYUP)
            {
                Config.LoadConfig();
                return true;
            }
        }

        if (inputHookClientNativeMessage.WParam == Config.ForwardsKey)
        {
            if ((PInvoke.User32.WindowMessage)inputHookClientNativeMessage.Type == PInvoke.User32.WindowMessage.WM_KEYDOWN)
            {
                double deltaTime = (DateTime.Now - lastFrameTime).TotalSeconds;
                Z -= Config.HorizontalSpeed * (float)deltaTime;
                return true;
            }
        }

        if (inputHookClientNativeMessage.WParam == Config.BackwardsKey)
        {
            if ((PInvoke.User32.WindowMessage)inputHookClientNativeMessage.Type == PInvoke.User32.WindowMessage.WM_KEYDOWN)
            {
                double deltaTime = (DateTime.Now - lastFrameTime).TotalSeconds;
                Z += Config.HorizontalSpeed * (float)deltaTime;
                return true;
            }
        }

        if (inputHookClientNativeMessage.WParam == Config.LeftKey)
        {
            if ((PInvoke.User32.WindowMessage)inputHookClientNativeMessage.Type == PInvoke.User32.WindowMessage.WM_KEYDOWN)
            {
                double deltaTime = (DateTime.Now - lastFrameTime).TotalSeconds;
                X += Config.HorizontalSpeed * (float)deltaTime;
                return true;
            }
        }

        if (inputHookClientNativeMessage.WParam == Config.RightKey)
        {
            if ((PInvoke.User32.WindowMessage)inputHookClientNativeMessage.Type == PInvoke.User32.WindowMessage.WM_KEYDOWN)
            {
                double deltaTime = (DateTime.Now - lastFrameTime).TotalSeconds;
                X -= Config.HorizontalSpeed * (float)deltaTime;
                return true;
            }
        }

        if (inputHookClientNativeMessage.WParam == Config.UpKey)
        {
            if ((PInvoke.User32.WindowMessage)inputHookClientNativeMessage.Type == PInvoke.User32.WindowMessage.WM_KEYDOWN)
            {
                double deltaTime = (DateTime.Now - lastFrameTime).TotalSeconds;
                Y += Config.VerticalSpeed * (float)deltaTime;
                return true;
            }
        }

        if (inputHookClientNativeMessage.WParam == Config.DownKey)
        {
            if ((PInvoke.User32.WindowMessage)inputHookClientNativeMessage.Type == PInvoke.User32.WindowMessage.WM_KEYDOWN)
            {
                double deltaTime = (DateTime.Now - lastFrameTime).TotalSeconds;
                Y -= Config.VerticalSpeed * (float)deltaTime;
                return true;
            }
        }

        return false;
    }

    ApiEntity.NativeHandle FetchPlayerEntity()
    {
        var currentApiWorld = V1.GetCApi1CurrentApiWorld();

        if (currentApiWorld == nint.Zero)
        {
            return (ApiEntity.NativeHandle)nint.Zero;
        }

        var nttUniverse = currentApiWorld.GetUniverse();

        if (nttUniverse == nint.Zero)
        {
            return (ApiEntity.NativeHandle)nint.Zero;
        }

        var playerControlSystem = NttUniverseSystemT__PlayerControlSystem.GetFrom(nttUniverse);

        if (playerControlSystem == nint.Zero)
        {
            return (ApiEntity.NativeHandle)nint.Zero;
        }

        return PlayerControlSystem.GetPlayerEntityForPlayerIdx(playerControlSystem, 0);
    }

    Vector3? FetchEntityPosition(ApiEntity.NativeHandle entity)
    {
        if (entity == nint.Zero)
        {
            return null;
        }

        var entityTransformComponent = (ApiTransformComponent.NativeHandle)entity.FindComponentByTypeName(
            ApiTransformComponent.Info.ApiClassName
        );

        if (entityTransformComponent == nint.Zero)
        {
            return null;
        }

        return entityTransformComponent.PositionNativeData.ToVector3();
    }

    void SetEntityPosition(ApiEntity.NativeHandle entity, float x, float y, float z)
    {
        if (entity == nint.Zero)
        {
            return;
        }

        var entityTransformComponent = (ApiTransformComponent.NativeHandle)entity.FindComponentByTypeName(
            ApiTransformComponent.Info.ApiClassName
        );

        if (entityTransformComponent == nint.Zero)
        {
            return;
        }

        entityTransformComponent.RotationNativeData = new Vector3(0f, 0f, 0f).ToVec3();
        entityTransformComponent.PositionNativeData = new Vector3(x, y, z).ToVec3();
    }

    void Update()
    {
        var player = FetchPlayerEntity();

        if (!noclipEnabled)
        {
            Vector3? vector = FetchEntityPosition(player);
            if (vector != null)
            {
                X = vector.Value.X;
                Y = vector.Value.Y;
                Z = vector.Value.Z;
            }
        }
        else
        {
            SetEntityPosition(player, X, Y, Z);
        }

        lastFrameTime = DateTime.Now;
    }

    public Noclip()
    {
        X = 0f;
        Y = 0f;
        Z = 0f;

        Config.LoadConfig();

        _inputHookClient = new InputHook1.Client(0, InputHookClientHandler);

        _gameFrameworkProcessMethodHook = new(
            GameFramework.ProcessMethod.Info.NativePtr,
            (nativeDataPtr) =>
            {
                try
                {
                    Update();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return _gameFrameworkProcessMethodHook!.Trampoline!(nativeDataPtr);
            }
        );

        _gameFrameworkProcessMethodHook.Enable();
    }
}