# WYUN Library
WYUN ServerにUnityから接続するためのライブラリです。  
一応Pure C#なはずなのでそれ以外のC#プロジェクトからも呼べると思います。

## 使い方
Unity内で使う方法のみ説明します。
### インストール
Package Managerからインストールしてください。  
[ローカルファイルからインストールする方法](../../wiki/インストール方法#ローカルファイルからインストールする方法)  
[ ~~リポジトリを指定してインストールする方法~~ ]()現在利用不可

### コード例
基本的な機能は`WYUN.Core`クラスからアクセスします。  
#### [接続情報の設定](../../wiki/WYUN.AppSettings)
```cs
WYUN.Core.setting=new WYUN.AppSetting(ipAddress as string,port as int,appID as long,playerName as string);
```
#### [サーバーへの接続](../../wiki/WYUN.Core/接続前#Connect)
```cs
WYUN.Core.Connect();
```
#### [ルームの作成と参加](../../wiki/WYUN.Core/ロビー内#CreateAndJoinRoom)
```cs
WYUN.Core.CreateAndJoinRoom(roomName as string,limit as int);
```
#### [ルームメンバーへのメッセージ](../../wiki/WYUN.Core/ルーム内#Broad)
```cs
WYUN.Core.Broad(message as string);
```
#### [メッセージの受信](../../wiki/WYUN.IRoomCallback#MessageReceived)
```cs
// IRoomCallbackを実装したクラス
public void MessageReceived(string message){
    string sender=message.Split(',')[0];
    string body=message.Split(',')[1];
    Debug.Log($"received message from {sender}: {body}");
}
```

## 注意事項
[`ILobbyCallback`](../../wiki/WYUN.ILobbyCallback)、[`IRoomCallback`](../../wiki/WYUN.IRoomCallback)で実装されるインターフェイス関数はすべてメインスレッド以外のスレッドから実行されています。  
`GameObject`の内部状態を変えるような操作は基本的にメインスレッドからのみ行えるため、適切にメインスレッドに処理を委譲する必要があります。  
回避策の一部は`Samples`のコードに実装されているので参考にしてください。

## ライセンス
MIT
## リンク
* [ドキュメント](../../wiki)
* [WYUN Server](https://github.com/ystt-lita/WYUN_Server)
* 
