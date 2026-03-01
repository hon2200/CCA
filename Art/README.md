## Git使用方法——写给美术
Git 是一个分布式版本控制系统，广泛用于代码管理和团队协作。下面是 Git 的常见使用方法：
### 本地安装 Git
下载地址： https://git-scm.com/download

下载完成后可以得到如下安装文件

<img width="173" height="23" alt="image" src="https://github.com/user-attachments/assets/bfffc85f-58dd-40a2-9fd1-952ace1a05be" />

这里默认下载的是64位的软件，双击下载的安装文件来安装Git。一直下一步直到安装完成即可。安装完成后在电脑桌面（也可以是其他目录）点击右键，如果能够看到如下两个菜单则说明Git安装成功。

<img width="213" height="151" alt="image" src="https://github.com/user-attachments/assets/2c0b6504-04ea-4be4-b776-c6e5a1d53011" />

#### 环境配置
当安装Git后首先要做的事情是设置用户名称和email地址。这是非常重要的，因为每次Git提交都会使用该用户信息

右键任意文件夹，点击Git Bash，执行以下指令：

设置用户信息

git config --global user.name “你的名字”

git config --global user.email “你的邮箱”

查看配置信息

git config --list

git config user.name

通过上面的命令设置的信息会保存在~/.gitconfig文件中

### 远程拉取仓库
