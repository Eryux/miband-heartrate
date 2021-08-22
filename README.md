# Mi Band Heartrate

Enable and monitor heartrate with Mi Band device on Windows 10.

![miband heartrate](https://github.com/Eryux/miband-heartrate/raw/master/mibandheatrate-screen.png "Mi Band Heartrate screen")

### Supported devices

* Mi Band 2
* Mi Band 3
* Mi Band 4


### Requirements

* .NET Framework 4.8 ([download](https://dotnet.microsoft.com/download/dotnet-framework/net48))
* Windows 10 1703+ (Creators Update)
* Bluetooth adapter supporting Bluetooth 4.0/4.2 BLE


### Usage

* Download and unzip the latest build from release section

#### For Mi Band 2 or Mi Band 3

* Un-pairing your Mi Band 2/3 from any devices

* Connect and pair your Mi Band 2/3 to your computer

* Launch `MiBand-Heartrate.exe`

* Click on `Connect` button and select your device from device list and set model on `Mi Band 2` or `Mi Band 3` then click on `Connect`

* Once your device is successfully connected and authenticated, click on `Start` button

#### For Mi Band 4

* Get your authentication key of your device, visit ([freemyband.com](http://www.freemyband.com/)) for more information

* Connect and pair you Mi Band 4 to your computer

* Launch `MiBand-Heartrate.exe`

* Click on `Connect` button and select your device from device list and set model on `Mi Band 4` then click on `Connect`

* A new window should appear, enter your authentication key then click on `Ok`

* Once your device is successfully connected and authenticated, click on `Start` button


### Options

* **Export data in CSV file :** Log your heartrate data into a CSV file with date, time and heartrate value

* **Write realtime date in text file :** Continuously write heartrate value inside a text file

* **Continuous mode :** Mi Band heartrate sensor can work in two different mode, one-shot and continuous. One-shot mode take 5 to 10 sec. to retrieve a heartrate value then stop. Continuous mode update heartrate value every 2 to 5 sec.


### Build requirements

* Windows SDK
* Visual Studio 2019


### Build

* Clone git repository

* Open `MiBand-Heartrate.sln` with Visual Studio

* Right-click on MiBand-Heartrate-2 solution and select generate


### Useful links

* [Microsoft GATT Documentation](https://docs.microsoft.com/fr-fr/windows/uwp/devices-sensors/bluetooth-low-energy-overview)
* [Mi Band 2 Authentication by leojrfs](https://leojrfs.github.io/writing/miband2-part1-auth/#reference), [python](https://github.com/leojrfs/miband2)
* https://github.com/creotiv/MiBand2
* [How I hacked my Xiaomi MiBand 2 fitness tracker — a step-by-step Linux guide by Andrey Nikishaev](https://medium.com/machine-learning-world/how-i-hacked-xiaomi-miband-2-to-control-it-from-linux-a5bd2f36d3ad)


### License

MIT License
