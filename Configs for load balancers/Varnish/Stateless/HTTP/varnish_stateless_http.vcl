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
    new vdir = directors.round_robin();
    vdir.add_backend(wcf_service_1);
    vdir.add_backend(wcf_service_2);
}

sub vcl_pipe {
    # This forces every request to the server to be closed.
    set bereq.http.connection = "close";
}

sub vcl_recv {
    set req.backend_hint = vdir.backend();
    #Disable caching
    return (pipe);
}