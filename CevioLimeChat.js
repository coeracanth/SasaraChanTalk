////////////////////////////////////////////////////////////////////////////////////////////////////
//■導入方法
// 1.当ファイルをLimeChatのscriptsフォルダに配置する
//   例）C:\【LimeChatインストール先】\users\【アカウント名】\scripts
//
// 2.LimeChat側でスクリプトを有効にする
//   ・LimeChatのメニューから「設定→スクリプトの設定」を開く。
//   ・スクリプトの設定画面で、セルを右クリックし、○を付ける。
//
////////////////////////////////////////////////////////////////////////////////////////////////////
//■設定

//発言者の名前を読み上げるかどうか(true:読む, false:読まない)
var bNick = true;

//入出情報を読み上げるかどうか(true:読む, false:読まない)
var bInOut = false;

////////////////////////////////////////////////////////////////////////////////////////////////////

var sRemoteTalkCmd = null;
var oShell;
var oWmi;

function addTalkTask(text) {
	if(sRemoteTalkCmd == null) {
		findRemoteTalk();
		if(sRemoteTalkCmd == null) {
			log("実行ファイルが見つからないのでスキップ-" + text);
			return;
		}
	}

	oShell.Run(sRemoteTalkCmd + " \"" + text.replace("\"", " ") + "\"", 0, false);
}

function talkChat(prefix, text) {
	if (bNick){
		addTalkTask(prefix.nick + "。" + text);
	} else {
		addTalkTask(text);
	}
}

function findRemoteTalk() {
	sRemoteTalkCmd = "\".\\AddStacker.exe\"";

	log("検出:" + sRemoteTalkCmd);
}

////////////////////////////////////////////////////////////////////////////////////////////////////

function event::onLoad() {
	oShell = new ActiveXObject("Wscript.Shell");
	oWmi   = GetObject("winmgmts:\\\\.\\root\\cimv2");

	//addTalkTask("ライムチャットとの連携を開始しました");
}

function event::onUnLoad() {
	oShell = null;
	oWmi   = null;

	//addTalkTask("ライムチャットとの連携を終了しました");
}

function event::onConnect(){
	addTalkTask(name + "サーバに接続しました");
}

function event::onDisconnect(){
	addTalkTask(name + "サーバから切断しました");
}

function event::onJoin(prefix, channel) {
	if (bInOut) {
		addTalkTask(prefix.nick + "さんが " + channel + " に入りました");
	}
}

function event::onPart(prefix, channel, comment) {
	if (bInOut) {
		addTalkTask(prefix.nick + "さんが " + channel + " から出ました。");
	}
}

function event::onQuit(prefix, comment) {
	if (bInOut) {
		addTalkTask(prefix.nick + "さんがサーバから切断しました。");
	}
}

function event::onChannelText(prefix, channel, text) {
	talkChat(prefix, text);
	//log("CnannelText[" + channel + "]" + text);
}

function event::onChannelNotice(prefix, channel, text) {
	talkChat(prefix, text);
	//log("CnannelNotice[" + channel + "]" + text);
}

function event::onChannelAction(prefix, channel, text) {
	talkChat(prefix, text);
	//log("CnannelAction[" + channel + "]" + text);
}

function event::onTalkText(prefix, targetNick, text) {
	talkChat(prefix, text);
	//log("TalkText[" + prefix.nick + "]" + text);
}

function event::onTalkNotice(prefix, targetNick, text) {
	talkChat(prefix, text);
	//log("TalkNotice[" + prefix.nick + "]" + text);
}

function event::onTalkAction(prefix, targetNick, text) {
	talkChat(prefix, text);
	//log("TalkAction[" + prefix.nick + "]" + text);
}

////////////////////////////////////////////////////////////////////////////////////////////////////
