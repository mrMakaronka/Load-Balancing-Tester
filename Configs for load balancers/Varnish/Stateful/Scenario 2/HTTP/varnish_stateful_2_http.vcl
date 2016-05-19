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
    new vdir = directors.hash();
    vdir.add_backend(wcf_service_1, 1.0); # 1.0 is the weight of the server
    vdir.add_backend(wcf_service_2, 1.0);
}

sub vcl_recv {
    set req.backend_hint = vdir.backend(req.url);
    #Disable caching
    return (pass);
}