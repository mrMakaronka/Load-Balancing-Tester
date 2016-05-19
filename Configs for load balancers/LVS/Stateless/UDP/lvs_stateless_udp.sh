sudo ipvsadm -A -u 192.168.1.1:8000 -s rr 
sudo ipvsadm -E -u 192.168.1.1:8000 --ops -s rr 

# -u means UDP
# rr means round-robin 
# --ops means one packet scheduling mode

sudo ipvsadm -a -u 192.168.1.1:8000 -r 192.168.1.2:8000 –m
sudo ipvsadm -a -u 192.168.1.1:8000 -r 192.168.1.3:8000 –m

# -m means NAT mode