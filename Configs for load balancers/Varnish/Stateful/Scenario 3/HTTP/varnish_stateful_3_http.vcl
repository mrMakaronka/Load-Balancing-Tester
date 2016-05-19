vcl 4.0;

import std;
import directors;

backend wcf_service_1 { 
    .host = "192.168.1.2";
    .port = "8000"; 
}

backend wcf_service_2 { 
    .host = "192.168.1.3";
    .port = "8000"; 
}

sub vcl_init {
    # It does not matter which scheduling method we use in this case
    new vdir1 = directors.round_robin();
    vdir1.add_backend(wcf_service_1);

    new vdir2 = directors.round_robin();
    vdir2.add_backend(wcf_service_2);
}

sub vcl_recv {
    # “serverid” is the url parameter name
    # “1” and “2” is the possible values
    if (req.url ~ "(?i)serverid=1") {
        set req.backend_hint = vdir1.backend();
    } else if (req.url ~ "(?i)serverid=2") {
        set req.backend_hint = vdir2.backend();
    } else {
        set req.backend_hint = vdir1.backend(); # default server
    }
    #Disable caching
    return (pass);
}