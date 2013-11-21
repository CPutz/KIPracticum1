@echo off

%~dp0..\Python\App\python.exe "%~dp0playgame.py" ^
 --engine_seed 42 ^
 --player_seed 42 ^
 --end_wait=0.25 ^
 --verbose ^
 -E -e ^
 --log_dir game_logs ^
 --turns 1000 ^
 --map_file "%~dp0maps\maze\maze_04p_01.map" ^
 "%~dp0..\your bot\bin\Debug\MyBot.exe" ^
 "%~dp0..\Python\App\python.exe ""%~dp0sample_bots\python\LeftyBot.py""" ^
 "%~dp0..\Python\App\python.exe ""%~dp0sample_bots\python\HunterBot.py""" ^
 "%~dp0..\Python\App\python.exe ""%~dp0sample_bots\python\RandomBot.py"""
