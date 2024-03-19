echo "Download count of all releases of Connect A PIC: "
curl -s https://api.github.com/repos/Akhetonics/Connect-A-PIC/releases | grep 'download_count' | awk -F ':' '{ sum += $2 } END { print sum }' | awk '{print $1}'
echo "Press enter to exit."
read