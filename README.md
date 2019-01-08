# Mi Band Heartrate

Enable and monitor heartrate with Mi Band 2 device on Windows 10.

![alt text](http://devhatch.fr/uploads/images/mibandheatrate-screen.png "Mi Band Heartrate screen")


### Requirements

* .NET Framework 4.5 ([download](https://www.microsoft.com/fr-fr/download/details.aspx?id=30653))
* Windows 10 1703+ (Creators Update)
* Bluetooth adapter supporting Bluetooth 4.0/4.2 BLE


### Usage

* Download and unzip the lastest build from release section

* Un-pairing your Mi Band 2 from any devices

* Pairing Mi Band 2 to your computer

* Launch `MiBand-Heartrate.exe`

* Select your device Mi Band 2 from the list and then connect (if no device showed, be sure to have un-paired and paired your device correctly, turned on the bluetooth adapter and the wifi).

* Click on lock for authenticate your device and push Mi Band 2 button

* Start monitoring heartrate by clicking on start button


### Options

* **Save data in CSV file :** Log your heart rate data into a CSV file with date and time and heart rate value

* **Write heartrate value in file :** Continuously write heart rate value in file

* **Continuous mode :** Mi Band 2 heart rate sensor can work in two different mode, one-shot and continuous. One-shot mode take ~5 sec. for retrieve a heart rate value and fails more often than continuous mode but the heart rate value is more precise. Continuous mode fails less and update more often but the value is less precise.


### Build requirements

* Windows SDK
* Visual Studio 2017


### Build

* Clone git repository

* Open `MiBand-Heartrate.sln` with Visual Studio

* Right-click on MiBand-Heartrate solution and select generate


### Useful links

* [Microsoft GATT Documentation](https://docs.microsoft.com/fr-fr/windows/uwp/devices-sensors/bluetooth-low-energy-overview)
* [Mi Band 2 Authentication by leojrfs](https://leojrfs.github.io/writing/miband2-part1-auth/#reference), [python](https://github.com/leojrfs/miband2)
* https://github.com/creotiv/MiBand2
* [How I hacked my Xiaomi MiBand 2 fitness tracker — a step-by-step Linux guide by Andrey Nikishaev](https://medium.com/machine-learning-world/how-i-hacked-xiaomi-miband-2-to-control-it-from-linux-a5bd2f36d3ad)


### License

MIT License