@echo off
chcp 65001 > nul
echo 批量Excel转JSON工具（第一行为列名）
echo 输入目录: .\Tables
echo 输出目录: .\Data

python ".\ExceltoJson.py" ^
".\Tables" ^
".\Data" ^
2 ^
0

if %errorlevel% equ 0 (
    echo 所有文件转换完成!
) else (
    echo 部分文件转换失败
)
pause
