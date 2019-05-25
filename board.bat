﻿"{Arduino_dir}arduino-builder" -dump-prefs -logger=machine -hardware "{Arduino_dir}hardware" -tools "{Arduino_dir}tools-builder" -tools "{Arduino_dir}hardware\tools\avr" -built-in-libraries "{Arduino_dir}libraries" -libraries "{lib_dir}" -fqbn=studuino:avr:studuino -ide-version=10808 -build-path "{build_dir}" -warnings=none -build-cache "{cache_dir}" -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avr-gcc.path="{Arduino_dir}hardware\tools\avr" -prefs=runtime.tools.avr-gcc-5.4.0-atmel3.6.1-arduino2.path="{Arduino_dir}hardware\tools\avr" -prefs=runtime.tools.avrdude.path="{Arduino_dir}hardware\tools\avr" -prefs=runtime.tools.avrdude-6.3.0-arduino14.path="{Arduino_dir}hardware\tools\avr" -prefs=runtime.tools.arduinoOTA.path="{Arduino_dir}hardware\tools\avr" -prefs=runtime.tools.arduinoOTA-1.2.1.path="{Arduino_dir}hardware\tools\avr" -verbose "{ino_file}"
"{Arduino_dir}arduino-builder" -compile -logger=machine -hardware "{Arduino_dir}hardware" -tools "{Arduino_dir}tools-builder" -tools "{Arduino_dir}hardware\tools\avr" -built-in-libraries "{Arduino_dir}libraries" -libraries "{lib_dir}" -fqbn=studuino:avr:studuino -ide-version=10808 -build-path "{build_dir}" -warnings=none -build-cache "{cache_dir}" -prefs=build.warn_data_percentage=75 -prefs=runtime.tools.avr-gcc.path="{Arduino_dir}hardware\tools\avr" -prefs=runtime.tools.avr-gcc-5.4.0-atmel3.6.1-arduino2.path="{Arduino_dir}hardware\tools\avr" -prefs=runtime.tools.avrdude.path="{Arduino_dir}hardware\tools\avr" -prefs=runtime.tools.avrdude-6.3.0-arduino14.path="{Arduino_dir}hardware\tools\avr" -prefs=runtime.tools.arduinoOTA.path="{Arduino_dir}hardware\tools\avr" -prefs=runtime.tools.arduinoOTA-1.2.1.path="{Arduino_dir}hardware\tools\avr" -verbose "{ino_file}"
"{Arduino_dir}hardware\tools\avr/bin/avrdude" -C"{Arduino_dir}hardware\tools\avr/etc/avrdude.conf" -v -patmega168 -carduino -P{comport} -b115200 -D -Uflash:w:"{build_dir}/{inoFileName}.hex":i 
