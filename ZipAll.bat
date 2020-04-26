cd ./Mods
for /D %%G in ("*") do if not "%%G" == "dist" (
	echo %%G
	7z a -tzip dist/%%G.zip %%G
)

pause