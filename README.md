# BKYFix（百科园 FixTool）

Windows 小工具，用于修复**百科园考试客户端**因缺少 .NET 3.5 或安装缓慢而无法正常启动的问题：在客户端目录下写入 **.NET Framework 4.8** 启动配置（`ExamClient.exe.config` 等）。

## 功能

- **自动侦测**：扫描本机快捷方式，定位考试客户端安装目录并批量修复配置。
- **拖放模式**：将「考试客户端」快捷方式（`.lnk`）拖入窗口，对指向目录执行修复。
- **环境检查**：启动时提示本机是否已安装 .NET Framework 4.8 / 4.8.1。

## 使用

1. 运行 `BKYFix.exe`（图形界面）。
2. 选择「自动侦测」或「拖放快捷方式」，按界面提示操作。
3. 查看下方日志，确认已写入配置或提示「已是 4.8 配置」。

### 命令行（可选）

```text
BKYFix.exe --apply-quiet "C:\路径\到\客户端目录"
BKYFix.exe --self-test
```

## 构建

- 需要 [.NET SDK](https://dotnet.microsoft.com/download)（项目目标框架：`net10.0-windows`）。
- 在仓库根目录执行：

```bash
dotnet build -c Release
```

## 说明

- 仅处理百科园相关客户端（如 `ExamClient.exe`、`ClientUpdate.exe` 等），不会修改无关程序。
- 修改配置文件可能需要管理员权限（工具会提示）。

## 许可证

未单独声明许可证时，版权归项目维护者所有。
