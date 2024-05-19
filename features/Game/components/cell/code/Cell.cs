using System;
using System.Collections.Generic;
using Godot;
using ThousandDevils.features.Game.components.cell.code.modules.logic;
using ThousandDevils.features.Game.components.field.code;
using ThousandDevils.features.Game.components.pawn.code;
using ThousandDevils.features.Game.utils;
using ThousandDevils.features.GlobalUtils;

namespace ThousandDevils.features.Game.components.cell.code;

// that interface just as documentation of available methods and properties
public interface ICell
{
  bool IsOpen { get; set; }
  bool IsHighlighted { get; set; }
  BaseLogic Logic { get; }
  Vector2I GridCords { get; set; }

  void AddPawn(Pawn pawn, bool callEvent = true);
  void RemovePawn(Pawn pawn, bool callEvent = true);
  IReadOnlyList<Pawn> GetPawns();

  event Action<Cell, Pawn> PawnWasAdded;
  event Action<Cell, Pawn> PawnWasRemoved;
  event Action<Cell, Pawn> WasDiscovered;
}

public partial class Cell : Node3D, ICell
{
  private bool _firstDiscovering = true;
  private MeshInstance3D _highlightBorder;

  // that elems was taken from GetChild or generated by _...CellScene
  private Node3D _invisibleCellMap;

  //value by default is child elem, can be changed by defined that properties
  [Export] private PackedScene _invisibleCellScene;

  private bool _isHighlighted;

  private bool _isOpen;
  private Node3D _visibleCellMap;
  [Export] private PackedScene _visibleCellScene;

  private List<Pawn> PawnsInside { get; } = new();

  public Field Field { get; private set; }

  // random by default and can be changed inside editor, if it still random - forceful change.
  [Export] public CellType Type { get; private set; } = CellType.Random;

  public bool CanAcceptPawns { get; set; } = true;

  public bool IsOpen {
    get => _isOpen;
    set {
      _isOpen = value;
      ChangeMapsVisibility(value);
      if (value && _firstDiscovering) {
        _firstDiscovering = false;
        WasDiscovered?.Invoke(this, GetPawns()[0]);
      }
    }
  }

  public bool IsHighlighted {
    get => _isHighlighted;
    set {
      _isHighlighted = value;
      _highlightBorder.Visible = value;
    }
  }

  // after type was generated, we should find and set logic based on Type.
  public BaseLogic Logic { get; private set; }

  public Vector2I GridCords { get; set; }

  public void AddPawn(Pawn pawn, bool callEvent = true) {
    if (PawnsInside.Contains(pawn)) return;
    PawnsInside.Add(pawn);
    AddChild(pawn);
    IsOpen = true;
    if (callEvent) PawnWasAdded?.Invoke(this, pawn);
  }

  public void RemovePawn(Pawn pawn, bool callEvent = true) {
    if (!PawnsInside.Contains(pawn)) return;
    PawnsInside.Remove(pawn);
    RemoveChild(pawn);
    if (callEvent) PawnWasRemoved?.Invoke(this, pawn);
  }

  public IReadOnlyList<Pawn> GetPawns() => PawnsInside.AsReadOnly();
  public event Action<Cell, Pawn> PawnWasAdded;
  public event Action<Cell, Pawn> PawnWasRemoved;
  public event Action<Cell, Pawn> WasDiscovered;

  [AssociateAttributes.ParentSetter]
  public void SetField(Field field) => Field = field;

  public override void _Ready() {
    base._Ready();
    _visibleCellMap = DefineMap(CellMapName.Visible, _visibleCellScene);
    _invisibleCellMap = DefineMap(CellMapName.Invisible, _invisibleCellScene);
    ChangeTypeIfRandom();
    ChangeMapsVisibility(false);
    _highlightBorder = GetNode<MeshInstance3D>("HighlightBorder");
    AdvancedReady();
  }

  //Calls callback when game gets based variables setup
  private void AdvancedReady() {
    Game.code.Game gameNode = GdUtilsFunctions.GetFirstChildByType<Game.code.Game>(GetTree().Root);
    if (gameNode != null) gameNode.AskForBasicReady(() => Logic = CreateLogicByType());
    else throw new MyExceptions.NotExistingElemException(typeof(Game.code.Game));
  }

  private void ChangeMapsVisibility(bool value) {
    _visibleCellMap.Visible = value;
    _invisibleCellMap.Visible = !value;
  }

  public void ReplaceCellMapScene(CellMapName name, PackedScene newScene) {
    if (name == CellMapName.Visible) {
      _visibleCellScene = newScene;
      _visibleCellMap = DefineMap(CellMapName.Visible, _visibleCellScene);
    }
    else {
      _invisibleCellScene = newScene;
      _invisibleCellMap = DefineMap(CellMapName.Invisible, _invisibleCellScene);
    }
  }

  private void ChangeTypeIfRandom() {
    if (Type != CellType.Random) return;
    Type = UtilsFunctions.GetRandomEnumValueExcluding(CellType.Random, CellType.Ocean, CellType.Ship, CellType.PossibleShip);
  }

  private Node3D DefineMap(CellMapName name, PackedScene sceneForMap) {
    Node3D resultMap;
    Node3D mapInCell = GetNode<Node3D>(name == CellMapName.Visible ? "VisibleMap" : "InvisibleMap");
    if (sceneForMap == null) {
      resultMap = mapInCell;
    }
    else {
      RemoveChild(mapInCell);
      resultMap = sceneForMap.Instantiate<Node3D>();
      AddChild(resultMap);
    }

    return resultMap;
  }

  private BaseLogic CreateLogicByType() {
    return Type switch {
      CellType.Ocean => new OceanLogic(this),
      CellType.Ice => new IceLogic(this),
      CellType.Arrow => new ArrowLogic(this),
      CellType.Ship => new ShipLogic(this),
      CellType.PossibleShip => new PossibleShipLogic(this),
      CellType.Trap => new TrapLogic(this),
      CellType.Crocodile => new CrocodileLogic(this),
      CellType.Ogre => new OgreLogic(this),
      CellType.Balloon => new BalloonLogic(this),
      CellType.RumTrap => new RumTrapLogic(this),
      CellType.Rum => new RumLogic(this),
      CellType.Jungles => new JunglesLogic(this),
      CellType.Fortress => new FortressLogic(this),
      CellType.Horse => new HorseLogic(this),
      CellType.Cannon => new CannonLogic(this),
      CellType.LightHouse => new LightHouseLogic(this),
      CellType.DreamFlora => new DreamFloraLogic(this),
      _ => new BaseLogic(this)
    };
  }
}
