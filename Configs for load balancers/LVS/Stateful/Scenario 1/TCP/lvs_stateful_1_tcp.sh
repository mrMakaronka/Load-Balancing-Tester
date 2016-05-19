sudo ipvsadm -A -t 192.168.1.1:8000 -s sh 

# -t means TCP
# sh means source hash

sudo ipvsadm -a -t 192.168.1.1:8000 -r 192.168.1.2:8000 –m
sudo ipvsadm -a -t 192.168.1.1:8000 -r 192.168.1.3:8000 –m

# -m means NAT mode