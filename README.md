# HPSocket.Net

### 介绍
the C# SDK for [HP-Socket](https://gitee.com/ldcsaa/HP-Socket)


### 安装教程
`HPSocket.Net`是通过NuGet软件包管理器交付的

在 Package Manager 控制台中使用以下命令来手动安装 `HPSocket.Net`
```
Install-Package HPSocket.Net
```
或在`Visual Studio`的解决方案中的`项目名`上`鼠标右键`->`管理 NuGet 程序包`->`浏览`页面->搜索框输入`HPSocket.Net`->然后安装

### 关于macOS
`HPSocket.Net`现在支持在`osx 10.12+`中使用`.net core2.0+`进行开发

Nuget软件包中的`libhpsocket4c.dylib`编译自`hp-socket`的`macOS分支`[HP-Socket-for-macOS](https://gitee.com/xin_chong/HP-Socket-for-macOS)


### 组件列表
#### 基础组件
基础组件是`hp-socket`提供的组件原型, 相关使用方法请参考[HP-Socket Development Guide.pdf](https://gitee.com/ldcsaa/HP-Socket/blob/master/Doc/HP-Socket%20Development%20Guide.pdf)

##### TCP
+ `ITcpServer`
+ `ITcpAgent`
+ `ITcpClient`
+ `ITcpPullServer`
+ `ITcpPullAgent`
+ `ITcpPullClient`
+ `ITcpPackServer`
+ `ITcpPackAgent`
+ `ITcpPacClient`

##### SSL
+ `ISslServer`
+ `ISslAgent`
+ `ISslClient`
+ `ISslPullServer`
+ `ISslPullAgent`
+ `ISslPullClient`
+ `ISslPackServer`
+ `ISslPackAgent`
+ `ISslPackClient`

##### HTTP
+ `IHttpServer`
+ `IHttpsServer`
+ `IHttpAgent`
+ `IHttpsAgent`
+ `IHttpClient`
+ `IHttpsClient`
+ `IHttpSyncClient`
+ `IHttpsSyncClient`

#### `HPSocket.Net`扩展组件
+ `IHttpEasyServer`
+ `IHttpsEasyServer`
+ `IHttpEasyAgent`
+ `IHttpsEasyAgent`
+ `IHttpEasyClient`
+ `IHttpsEasyClient`
+ `IWebSocketServer`
+ `IWebSocketAgent` 

`HPSocket.Net`暂时提供6个Easy组件和2个WebSocket组件, 用来更简单的处理http/https/ws的数据包, `hp-socket`提供的基础组件, 需要自己来实现数据包的完整获取, Easy组件已经做了这些处理, http/https的Easy组件绑定以下事件, 当事件到达, 即可获得完整数据包
 
+ `OnEasyChunkData` Chunk消息的完整数据包事件
+ `OnEasyMessageData` get或post的完整数据包事件
+ `OnEasyWebSocketMessageData` WebSocket消息的完整数据包事件

`WebSocket` 请直接使用以下两个组件

+ `IWebSocketServer` WebSocket 服务端
+ `IWebSocketAgent` WebSocket 客户端, 不同其他agent组件, WebSocket的agent组件不支持连接到不同的WebSocket server, 也就是说`IWebSocketAgent`组件所有的连接都是针对同一个服务器的


### 使用说明
1. 大部分组件使用方法清参考`demo`目录下的工程
2. `HPSocket.Net`扩展组件事件绑定
#### IHttpEasyServer
```cs
// 创建 HttpEasyServer 的实例
using(IHttpEasyServer httpServer = new HttpEasyServer())
{
    // ...其他设置

    // 绑定 OnEasyMessageData 事件
    httpServer.OnEasyMessageData += (sender, id, data) => 
    {
        // data 参数每次都是一个完整的数据包
        // ... 处理 data

        return HttpParseResult.Ok;
    };
}
```

#### IHttpEasyAgent
```cs
// 创建 HttpEasyAgent 的实例
using(IHttpEasyAgent httpAgent = new HttpEasyAgent())
{
    // ...其他设置

    // 绑定 OnEasyMessageData 事件
    httpAgent.OnEasyMessageData += (sender, id, data) => 
    {
        // data 参数每次都是一个完整的数据包
        // ... 处理 data

        return HttpParseResult.Ok;
    };
}
```

#### IHttpEasyClient
```cs
// 创建 HttpEasyClient 的实例
using(IHttpEasyClient httpClient = new HttpEasyClient())
{
    // ...其他设置

    // 绑定 OnEasyMessageData 事件
    httpClient.OnEasyMessageData += (sender, data) => 
    {
        // data 参数每次都是一个完整的数据包
        // ... 处理 data

        return HttpParseResult.Ok;
    };
}
```

### 参与贡献

1.  Fork 本仓库
2.  新建 Feat_xxx 分支
3.  提交代码
4.  新建 Pull Request
