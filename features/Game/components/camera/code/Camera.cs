using System;
using System.Collections.Generic;
using Godot;
using ThousandDevils.features.Game.components.camera.code.modules.modes;
using ThousandDevils.features.Game.utils;
using ThousandDevils.features.GlobalUtils;

namespace ThousandDevils.features.Game.components.camera.code;

public partial class Camera : Camera3D
{
  private readonly Dictionary<CameraModeType, BaseMode> _modes = new();
  public BaseMode CurrentMode { get; private set; }

  public Game.code.Game Game { get; private set; }

  [AssociateAttributes.ParentSetter]
  public void UpdateGame(Game.code.Game game) => Game = game;

  public override void _Ready() {
    base._Ready();
    _modes.Add(CameraModeType.Free, new FreeMode(this));
    _modes.Add(CameraModeType.VerticalPinned, new VerticalPinnedMode(this));
    UpdateCameraMode(CameraModeType.Free);
    CurrentMode?.OnReady();
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);
    CurrentMode?.OnInput(@event);
  }

  public override void _Process(double delta) {
    base._Process(delta);
    CurrentMode?.OnProcess(delta);
  }

  public void UpdateCameraMode(CameraModeType modeType) {
    CurrentMode = _modes[modeType];
    OnModeChange?.Invoke(CurrentMode);
  }

  public event Action<BaseMode> OnModeChange;
}