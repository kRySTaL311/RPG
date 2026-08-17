# RPG

一款基于 **Unity 6**（`6000.4.7f1`）开发的 **2D 像素风角色扮演游戏**。

---

## 🕹️ 操作

| 操作 | 按键 |
| --- | --- |
| 移动 | `WASD` / 方向键 |
| 交互（对话 / 开门 / 拾取 / 招募） | `E` |
| 打开背包 | `I` |
| 音量面板 | `V` / `Esc` |
| 调试：手动存档 | `P` |
| 调试：清除存档 | `M` |

> 移动使用 Unity 旧版 Input Manager 的 `Horizontal` / `Vertical` 轴。

---

## 🎮 游戏内容

- **场景探索** —— 玩家移动、镜头跟随、位置记忆（再次进入恢复上次位置）
- **NPC 对话** —— 靠近后按 `E` 触发对话
- **世界交互** —— 门与场景切换、机关触发、物品拾取
- **背包与物品** —— 按 `I` 打开，查看物品与详情
- **锻造合成** —— 通过配方消耗材料合成物品
- **任务系统** —— 领取 / 进行 / 完成的任务流程，带任务 UI
- **金币** —— 全局金币管理
- **队伍与编队** —— 上场队伍 + 替补席，可招募新的 NPC 入队
- **回合制战斗** —— 靠近敌人进入战斗（`Battle` 场景）
  - 普通攻击 / 技能施放（消耗法力）
  - 暴击与中毒效果
  - **自动战斗**：开启后自动进行，可用倍速循环切换 `1x → 2x → 3x`
  - 战斗结束结算界面
- **存档** —— 基于 `PlayerPrefs`（队伍、角色、背包、金币、场景进度等）

---

## 🚀 快速开始

1. 用 **Unity Hub** 打开项目（Unity `6000.4.7f1` 或兼容的 Unity 6 版本）。
2. 等待首次导入完成（`Library`、`.csproj` 等已在 `.gitignore` 中忽略）。
3. 打开场景 `Assets/Scenes/Menu.unity`。
4. 点击 **Play**，在主菜单点「New Game」进入游戏。

> 场景流程：**Menu（主菜单）→ Level 1（主关卡，含探索 / 任务 / 战斗触发）→ Battle（战斗）**。

---

## 📁 项目结构

```
Assets/
├── Scenes/           场景（Menu / Level 1 / Battle）
├── Scripts/          脚本（按功能分类）
│   ├── Player/       玩家控制、镜头
│   ├── Combat/       战斗、敌人、法术、编队
│   ├── Characters/   角色、数据、队员、可招募 NPC
│   ├── Items/        背包、物品、锻造配方
│   ├── Quests/       任务
│   ├── World/        对话、门、拾取、机关
│   ├── UI/           UI（菜单、战斗、物品、任务等）
│   ├── Audio/        音频、BGM、音量
│   ├── Effects/      淡入淡出、屏幕震动、飘字
│   └── Systems/      存档、金币、队伍
├── Sprites/          全部精灵图（按来源分子目录）
├── Prefabs/          预制体
├── Resources/        运行时加载的资源（角色数据 / 物品 / 配方）
├── Input/            Input System 动作资产与生成类
└── TextMesh Pro/     TMP 字体与设置
```

---

## 🧰 技术栈

- **Unity 6000.4.7f1**（Unity 6，URP 2D 渲染管线）
- **C#**，旧版 **Input Manager**
- **TextMesh Pro**（TMP）文字渲染
- 素材来源：Epic RPG World、Monster、ssssppp、Free-Basic-Pixel-Art-UI 等素材包

---

## 📌 说明

- 存档位于 `PlayerPrefs`，删除即清除存档。
- 已配置 `.gitignore`（忽略 `Library`、`Temp`、构建产物、备份目录与 Mac 归档垃圾）。
