# McPacketDisplay
I've been finding using WireShark to examine MineCraft b1.7.3 packets to be rather onerous.
One has to interpret everything from a hex dump, and manually find boundaries between 
the multiple MineCraft packets that may be contained in a single TCP packet.  This is time
consuming and error prone.

So I'm creating this tool to help me analyze the traffic.  At the moment, it
does not support any packets with a variable length (except string types).
The TCP filters don't work, so it relies on filtering when capturing the TCP Dump.
