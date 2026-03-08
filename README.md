# 文件夹管理
**重要：注意事项**
1. 引入外部资源时，如果是代码资源放到Plugin里面，如果是美术资源（我们自己做的资源不算）放到Resource/ThirdParty里面，不要破坏原本的代码结构
2. 不要用中文命名文件夹
3. 上传文件的时候，不要上传无关或者Unity自带的文件！大多数情况只需要在Assets文件夹内部Git Bash

## Art
存放美术的资源，但是用到项目中的还是放在Src里面的Art文件夹，现在没人用

## Doc
存放解释文档，现在没人用

## Audio
存放音频资源，但是用到项目中还是放在Src里面的Audio文件夹，现在没人用

## Src
存放项目源码，也是Unity项目的主文件夹

### Assets
核心资源

#### Editor
开发用的一些小工具，这里面的代码在运行时不会编译，可以安全地引用UnityEditor

#### Log
输出日志

#### Plugins
依赖的第三方解决方案，目前主要是SerializedDictionary

#### Resources
美术资源，点开Resources文件夹里面的Readme可以查看具体管理方式

#### Scene
游戏场景

#### Scripts
游戏代码，点开Scipts文件夹里面的Readme可以查看具体管理方式

#### Settings
Unity渲染管线的一些文件放在这里，由于不清楚引用原理，我不太敢轻易动这里面的东西

#### StreamingAssets
存放游戏数据，主要是表格和Json文件
维护方式：修改Common/Tables/Tables里面的表格，点击运行Common/Tables/ExceltoJson.py，如有需要调整这个脚本，将表格以需要的格式转化成Common/Tables/Data里面的内容，再由游戏中的相关脚本去读取

#### TextMeshPro
这个不需要解释

### Packages
一些额外的包会上传到Github，目前就NewtonSoft.Json

### ProjectSettings
有些项目设置需要在这里同步，目前会动的就TagManager，管理UI图层，需要同步

# 喵
## 喵喵
### 喵喵喵
