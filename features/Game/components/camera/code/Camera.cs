using System;
using Godot;
using ThousandDevils.features.Game.components.camera.code.modules.modes;
using ThousandDevils.features.Game.utils;
using ThousandDevils.features.GlobalUtils;

namespace ThousandDevils.features.Game.components.camera.code;

public class Camera : Camera3D
{
  private CameraModeType _modeType = CameraModeType.Free;
  public Game.code.Game Game { get; private set; }
  public BaseMode CameraMode { get; private set; }

  public CameraModeType ModeType {
    get => _modeType;
    set {
      _modeType = value;
      OnModeChange?.Invoke(_modeType);
    }
  }

  public void AskForRayCast(Action<Node> onRayCastCallback, Predicate<Node> predicate = null, bool pauseDefaultRayCast = true) {
    if (pauseDefaultRayCast) CameraMode.ProcessDefaultRayCast = false;

    void Wrapper(Node node) {
      if (predicate != null && !predicate(node)) return;
      onRayCastCallback(node);
      CameraMode.OnRayCast -= Wrapper;
      if (pauseDefaultRayCast) CameraMode.ProcessDefaultRayCast = true;
    }

    CameraMode.OnRayCast += Wrapper;
  }

  [AssociateAttributes.ParentSetter]
  private void UpdateGame(Game.code.Game game) => Game = game;

  public override void _Ready() {
    base._Ready();
    UpdateCameraLogicByType(_modeType);
    OnModeChange += UpdateCameraLogicByType;
    CameraMode?.OnReady();
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);
    CameraMode?.OnInput(@event);
  }

  public override void _Process(double delta) {
    base._Process(delta);
    CameraMode?.OnProcess(delta);
  }

  private void UpdateCameraLogicByType(CameraModeType modeType) {
    switch (modeType) {
      case CameraModeType.Free:
        CameraMode = new FreeMode(this);
        break;
      case CameraModeType.Fixed:
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(modeType), modeType, null);
    }
  }

  public event Action<CameraModeType> OnModeChange;
}
