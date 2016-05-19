sudo ipvsadm -A -t 192.168.1.1:8000 -s rr 

# -t means TCP
# rr means round-robin 

sudo ipvsadm -a -t 192.168.1.1:8000 -r 192.168.1.2:8000 –m
sudo ipvsadm -a -t 192.168.1.1:8000 -r 192.168.1.3:8000 –m

# -m means NAT mode