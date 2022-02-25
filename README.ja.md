# Mi Band Heartrate

Windows10でMi Bandデバイスで心拍数を取得します。

![miband heartrate](https://github.com/Eryux/miband-heartrate/raw/master/mibandheatrate-screen.png "Mi Band Heartrate screen")

### サポートしているデバイス

* Mi Band 2
* Mi Band 3
* Mi Band 4
* Mi Band 5


### 動作環境

* .NET Framework 4.8 ([download](https://dotnet.microsoft.com/download/dotnet-framework/net48))
* Windows 10 1703+ (Creators Update)
* Bluetooth adapter supporting Bluetooth 4.0/4.2 BLE


### 使い方

* 最新のリリースからzipをダウンロード・解凍してください。

#### Mi Band 2 or Mi Band 3

* Mi Band 2/3を全てのデバイスからペアリング解除してください

* Mi Band 2/3をPCにペアリング・接続してください

* `MiBand-Heartrate.exe`を起動してください

* `Connect`ボタンを押してください

* `Select your device`から`Mi Band 2`や`Mi Band 3`を選択してください

* `Select the model`から`Mi Band 2/3`を選択してください

* `Connect`ボタンを押してください

* デバイスが正しく接続・認証されました、`Start`ボタンを押してください

#### Mi Band 4/5

* デバイス用の認証キーを取得してください、詳しくは ([freemyband.com](http://www.freemyband.com/))

* Mi Band 4/5をPCにペアリング・接続してください

* `MiBand-Heartrate.exe`を起動してください

* `Connect`ボタンを押してください

* `Select your device`から`Mi Band 4`や`Mi Band 5`を選択してください

* `Select the model`から`Mi Band 4/5`を選択してください

* `Connect`ボタンを押してください

* 新しいウィンドウが出るので、そこに認証キーを入力して`Ok`を押してください

* デバイスが正しく接続・認証されました、`Start`ボタンを押してください


### オプション

* **Export data in CSV file :** 日付、時刻、心拍数を含むCSVファイルに記録します。

* **Write realtime date in text file :** テキストファイル内に心拍数の値を継続的に書き込みます。

* **Send OSC to VRChat :** OSCを使用して心拍や心拍数を送信します。詳細な仕様は次の章を参照してください。

* **Continuous mode :** Mi Bandの心拍センサーは「1回のみ取得」「連続取得」の2個のモードがあります。「1回のみ取得」のモードでは取得まで5\~10秒かかり、心拍数の値を取得してから停止します。「連続取得」のモードでは、2\~5秒ごとに心拍数の値が更新されます。

## VRChat
### Addresses

"Send OSC to VRChat"が有効になっている場合はOSCを送信し続けます。

|Addresss|Value Type|Description|
|-|-|-|
|/avatar/parameters/HeartRateInt|Int|心拍数（毎分） [0, 255]|
|/avatar/parameters/HeartRateFloat|Float|正規化された心拍数（毎分） ([0, 255] -> [0, 1]) <br> これはラジアルを使用したシェイプキーの操作をするときに有効です。[参考リンク](https://note.com/citron_vr/n/n7d54ebaebd83)|
|/avatar/parameters/HeartBeatInt|Int|1 : QRS時間（ドックンの時間）(心拍の1/5の時間としています) <br> 0 : それ以外の時間|
|/avatar/parameters/HeartBeatPulse|Bool|True : QRS時間（ドックンの時間）(心拍の1/5の時間としています) <br> False : それ以外の時間|
er timesarameters/HeartBeatToggle|Bool|心拍ごとに値が反転します|

### ビルドに必要なもの

* Windows SDK 10.0.18362.1

  ダウンロード : [Windows SDK and emulator archive](https://developer.microsoft.com/en-US/windows/downloads/sdk-archive/)
* Visual Studio 2019


### Build

* このリポジトリをクローンしてください

* `MiBand-Heartrate.sln`のソリューションを開いてください

* MiBand-Heartrate-2のソリューションを右クリックしてビルド


### 便利なリンク

* [Microsoft GATT Documentation](https://docs.microsoft.com/fr-fr/windows/uwp/devices-sensors/bluetooth-low-energy-overview)
* [Mi Band 2 Authentication by leojrfs](https://leojrfs.github.io/writing/miband2-part1-auth/#reference), [python](https://github.com/leojrfs/miband2)
* https://github.com/creotiv/MiBand2
* [How I hacked my Xiaomi MiBand 2 fitness tracker — a step-by-step Linux guide by Andrey Nikishaev](https://medium.com/machine-learning-world/how-i-hacked-xiaomi-miband-2-to-control-it-from-linux-a5bd2f36d3ad)
* [ラジアルでのシェイプキー操作方法｜みかんねここ｜note](https://note.com/citron_vr/n/n7d54ebaebd83)


### Thirdparty licenses
Rug.Osc | [MIT Licence](https://bitbucket.org/rugcode/rug.osc/wiki/License)


### License

MIT License
