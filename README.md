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
* [How To Use Android BLE to Communicate with Bluetooth Devices - An Overview & Code examples by Shahar Avigezer](https://medium.com/@avigezerit/bluetooth-low-energy-on-android-22bc7310387a)


### License

MIT License