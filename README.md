# RPG

基于 **Unity 6**（`6000.4.7f1`）开发的 **2D 像素风海贼题材角色扮演游戏**。

包含回合制战斗、队伍养成、装备锻造、任务与对话系统，支持键盘与手柄操作。

---

## ✨ 功能特性

- **回合制战斗** —— 普通攻击 / 魔法技能、暴击与中毒效果、战斗结果结算
  - **自动战斗**：一键开启自动战斗 + 多档倍速（1x / 2x / 4x）
  - **技能面板**：按法力消耗施放单体 / 群体（AoE）/ 治疗技能
- **队伍系统** —— 上场队伍 + 替补席编队（`TeamManager` / `FormationManager`），角色招募（`RecruitableNPC`）
- **角色养成** —— 等级 / 经验 / 生命 / 攻击 / 防御 / 法力 / 暴击率 / 暴击倍率 / 中毒伤害（`CharacterData`）
- **背包与装备** —— 物品栏管理、装备属性预览、物品详情面板
- **锻造系统** —— 通过配方合成装备（`CraftingRecipe` + `CraftingWindow`）
- **任务系统** —— 任务状态机（未接取 / 进行中 / 完成）、任务发放 NPC 与任务 UI
- **NPC 对话** —— 对话触发与对话流程控制（`DialogController` / `DialogTrigger`）
- **存档系统** —— 基于 `PlayerPrefs`，保存队伍、角色、背包、金币、场景进度等
- **金币经济** —— 全局金币管理与持久化（`GoldManager`）
- **世界交互** —— 门与场景切换（`DoorManager`）、物品拾取（`Pickup`）、机关触发（`ObjectActivator`）
- **音效与音乐** —— 全局音频管理、场景 BGM、音量调节（`AudioManager` / `SceneMusic` / `VolumeController`）
- **表现效果** —— 场景淡入淡出、屏幕震动、飘字伤害（`FadeController` / `ScreenShakeManager` / `FloatingDamageText`）

---

## 🕹️ 操作

| 操作 | 键盘 | 手柄 |
| --- | --- | --- |
| 移动 | `WASD` / 方向键 | 左摇杆 |
| 交互 / 对话 | `E` | 手柄主键 |
| 攻击 | 鼠标左键 | 手柄主键 |
| 跳跃 | `空格` | 手柄南键 |
| 下蹲 | `C` | — |
| 角色切换 | `1` / `2` | 十字键左右 |

> 完整按键配置见 `Assets/Input/InputSystem_Actions.inputactions`（新 Input System）。

---

## 🚀 快速开始

1. 用 **Unity Hub** 打开项目（Unity `6000.4.7f1` 或兼容的 Unity 6 版本）。
2. 等待首次导入完成（会生成 `Library`、`.csproj` 等，已在 `.gitignore` 中忽略）。
3. 打开场景 `Assets/Scenes/Menu.unity`。
4. 点击 **Play** 开始游戏。

> 场景流程：**Menu（主菜单）→ Level 1（关卡探索）→ Battle（战斗）**。

---

## 📁 项目结构

```
Assets/
├── Scenes/           场景（Menu / Level 1 / Battle）
├── Scripts/          脚本（按功能分类）
│   ├── Player/       玩家控制、镜头
│   ├── Combat/       战斗控制器、敌人、法术、编队
│   ├── Characters/   角色基类、数据、队员、可招募 NPC
│   ├── Items/        背包、物品数据、锻造配方
│   ├── Quests/       任务系统
│   ├── World/        对话、门、拾取、机关
│   ├── UI/           UI 管理器（菜单、战斗、物品、任务等）
│   ├── Audio/        音频管理、BGM、音量
│   ├── Effects/      淡入淡出、屏幕震动、飘字
│   └── Systems/      存档、金币、队伍
├── Sprites/          全部精灵图（按来源分子目录）
├── Prefabs/          预制体
├── Resources/        运行时按路径加载的资源（角色数据 / 物品 / 配方）
├── Input/            Input System 动作资产与生成类
├── TextMesh Pro/     TMP 字体与设置
└── SIMKAI SDF.asset  全局字体（楷体 SDF）
```

---

## 💾 存档

存档基于 **`PlayerPrefs`**（本机注册表 / 配置文件），涉及：

- `SaveManager` —— 队伍、角色数据、背包、金币等
- `CharacterSaveManager` —— 单个角色的独立存档
- `ZoneSaveManager` —— 场景 / 区域进度
- `GoldManager` —— 金币

删除 `PlayerPrefs`（或在游戏内重置）可清除存档。

---

## 🧰 技术栈

- **Unity 6000.4.7f1**（Unity 6，URP 2D 渲染管线）
- **C#**，新 **Input System**
- **TextMesh Pro**（TMP）文字渲染
- 主要素材来源：Epic RPG World、Monster、ssssppp、Free-Basic-Pixel-Art-UI 等素材包

---

## 📌 说明

- 本项目已配置 `.gitignore`（忽略 `Library`、`Temp`、构建产物、备份目录与 Mac 归档垃圾）。
- 备份文件夹（`_*backup_*`）不纳入版本管理，确认后可删除。
