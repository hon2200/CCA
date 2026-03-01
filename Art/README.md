## Git使用方法——写给美术
Git 是一个分布式版本控制系统，广泛用于代码管理和团队协作。下面是 Git 的常见使用方法：
### 第一步：本地安装 Git
下载地址： https://git-scm.com/download

- **Windows**：下载并安装 [Git for Windows](https://git-scm.com/download/win)。
- **Mac**：使用 Homebrew 安装：
  ```bash
  brew install git
  ```

* **Linux**：使用包管理器安装，例如在 Ubuntu 上：

  ```bash
  sudo apt-get install git
  ```

下载完成后可以得到如下安装文件

<img width="173" height="23" alt="image" src="https://github.com/user-attachments/assets/bfffc85f-58dd-40a2-9fd1-952ace1a05be" />

这里默认下载的是64位的软件，双击下载的安装文件来安装Git。一直下一步直到安装完成即可。安装完成后在电脑桌面（也可以是其他目录）点击右键，如果能够看到如下两个菜单则说明Git安装成功。

<img width="213" height="151" alt="image" src="https://github.com/user-attachments/assets/2c0b6504-04ea-4be4-b776-c6e5a1d53011" />

#### 环境配置
当安装Git后首先要做的事情是设置用户名称和email地址。这是非常重要的，因为每次Git提交都会使用该用户信息

右键任意文件夹，点击Git Bash，执行以下指令：

设置用户信息

```bash
git config --global user.name “你的名字”
git config --global user.email “你的邮箱”
```

查看配置信息

```bash
git config --list
git config user.name
```

通过上面的命令设置的信息会保存在~/.gitconfig文件中

### 第二步：注册Github账号
这一步相对简单，访问Github官方网站（可能需要梯子）：https://github.com

按照引导完成注册即可

### 第三步：远程拉取仓库

可以通过Git提供的命令从远程仓库进行克隆，将远程仓库克隆到本地

命令形式为：git clone 远程仓库地址

在你想要放置CCA的文件夹中，右键Git Bash，输入以下命令

拉取方式1：SSH协议，需要先配置SSH协议，如果不想麻烦可以使用方式2
```bash
git clone git@github.com:hon2200/CCA.git
```
拉取方式2：HTTP协议
```bash
git clone https://github.com/hon2200/CCA.git
```

#### SSH协议配置说明
要通过 SSH 协议从 GitHub 拉取（clone）远程仓库，需要以下几个步骤和要求：

##### 1.生成 SSH 密钥对

如果你还没有 SSH 密钥，你需要生成一对新的 SSH 密钥。

生成 SSH 密钥对：

打开终端（Terminal）或命令行工具。

执行以下命令来生成一个新的 SSH 密钥对（请确保替换邮箱地址为你自己的 GitHub 账户邮箱）：

```bash
ssh-keygen -t rsa -b 4096 -C "your_email@example.com"
```

当系统提示你选择文件保存位置时，直接按 Enter 以使用默认位置（`~/.ssh/id_rsa`），或者指定一个路径。

你会被要求输入一个 passphrase（密码短语），这是一个额外的安全层，但也可以直接按 Enter 跳过。

生成后，你的 SSH 密钥对将保存在 `~/.ssh/` 目录下，包含两个文件：

`id_rsa`：私钥（保密，不可泄露）

`id_rsa.pub`：公钥（需要添加到 GitHub）

##### 2.将 SSH 公钥添加到 GitHub 账户

复制你的 SSH 公钥内容。你可以使用以下命令来查看并复制公钥：

```bash
cat ~/.ssh/id_rsa.pub
```

登录到 GitHub 账户，在右上角点击 头像，然后选择 Settings（设置）。

在左侧栏中，选择 SSH and GPG keys。

点击 New SSH key，然后在弹出的页面中：

在 Title 中为该密钥取个名字（例如："My Laptop SSH"）。

将你复制的公钥粘贴到 Key 输入框中。

点击 Add SSH key 完成添加。

##### 3.测试 SSH 连接

确保 GitHub 正确地识别了你的 SSH 密钥，可以通过以下命令测试连接：

```bash
ssh -T git@github.com
```

你应该会看到类似以下的输出：

```bash
Hi username! You've successfully authenticated, but GitHub does not provide shell access.
```

这表示你的 SSH 密钥成功地与 GitHub 账户配对。

### 4.同步与提交更改

#### 查看仓库状态

查看当前文件的状态，哪些文件已修改或新添加：

```bash
git status
```

#### 添加与提交更改

在修改文件后，使用 `git add` 将文件添加到暂存区：

```bash
git add <filename>       # 添加单个文件
git add .                # 添加当前目录下所有改动的文件
```

一旦文件被添加到暂存区，可以使用 `git commit` 提交更改：

```bash
git commit -m "Your commit message"
```

提交信息应该简洁明了，描述这次提交做了什么更改。

将本地的更改推送到远程仓库：

```bash
git push origin <branch_name>
```

`<branch_name>`是分支的名字，通常为主分支`master`，通常情况下
```bash
git push
```
就可以满足需求

**注意：除Art文件夹外，美术不应push对其他文件夹内容的更改**

## 12. 拉取远程更改

如果有其他人对远程仓库进行了修改，可以使用以下命令拉取最新更改：

```bash
git pull origin <branch_name>
```

这会把远程仓库 `<branch_name>` 分支的更改拉取到本地并合并。

通常情况下
```bash
git pull
```
就可以满足需求


#### 查看提交历史和文件更改

查看项目的提交历史：

```bash
git log
```

你可以使用 `git log --oneline` 以简洁格式查看历史记录。

查看文件的具体修改内容：

```bash
git diff
```

#### 分支管理（对美术来说不太重要）

Git 支持分支管理，常见的分支命令包括：

* **创建新分支**：

  ```bash
  git branch <branch_name>
  ```

* **切换分支**：

  ```bash
  git checkout <branch_name>
  ```

* **创建并切换到新分支**：

  ```bash
  git checkout -b <branch_name>
  ```

* **查看所有分支**：

  ```bash
  git branch
  ```

* **合并一个分支到当前分支**：

  ```bash
  git merge <branch_name>
  ```

## 12. 拉取远程更改

如果有其他人对远程仓库进行了修改，可以使用以下命令拉取最新更改：

```bash
git pull origin <branch_name>
```

这会把远程仓库 `<branch_name>` 分支的更改拉取到本地并合并。

## 14. 删除本地分支

删除不再需要的本地分支：

```bash
git branch -d <branch_name>
```

## 15. 删除远程分支

如果远程分支不再需要，可以删除远程分支：

```bash
git push origin --delete <branch_name>
```

## 16. 恢复更改

如果你在文件中做了改动，但想撤销这些修改，可以使用以下命令：

* **撤销未提交的更改**（恢复到上次提交状态）：

  ```bash
  git checkout -- <filename>
  ```

* **撤销暂存区的更改**（撤销 `git add`）：

  ```bash
  git reset <filename>
  ```

* **恢复已删除的文件**：

  ```bash
  git checkout -- <filename>
  ```

## 17. 回滚到之前的版本

如果你想回滚到某个特定的提交，可以使用 `git reset` 或 `git revert`：

* **回滚到指定的 commit（丢弃之后的更改）**：

  ```bash
  git reset --hard <commit_id>
  ```

* **撤销某次提交并保留更改**：

  ```bash
  git reset <commit_id>
  ```

* **创建一个新的提交来撤销某次提交的更改**：

  ```bash
  git revert <commit_id>
  ```

## 18. 查看远程仓库

查看远程仓库的配置：

```bash
git remote -v
```

## 19. 添加远程仓库

如果你还没有添加远程仓库，可以使用以下命令：

```bash
git remote add origin https://github.com/username/repository.git
```

## 20. 删除远程仓库

删除远程仓库配置：

```bash
git remote remove origin
```

```

将此内容复制并保存为 `.md` 文件，例如 `Git_Usage.md`。这样你就可以随时查看和使用这些 Git 命令了！
```

