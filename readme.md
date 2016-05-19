# Load Balancing Tester
## Tool description

The main purpose of the tool is to check if your load balancing solution is set up correctly to support:

 * Uniform distribution for stateless scenario
 * Different protocols (HTTP, TCP, UDP)
 * SSL (termination or pass-through)
 * Sessions (scenarios are described below)
 * WCF features

The tool covers the following load balancing scenarios:
1. Stateless services
	* All requests are uniformly distributed across the server instances
2. Stateful services
	* All sessions from the same client are handled by the same server. We assume that clients are the same if they have the same IP address
	* Each session is handled by a random server (implemented using extra URL parameter)
	* Client chooses which server should process current session. The next session can be processed by another server on request (implemented using extra URL parameter).

The project contains a lot of pre-defined WCF bindings, which cover many test cases including different types of protocols, security connection scenarios, and WCF features. By adding additional bindings, the tool can be configured for custom test cases. 

Steps to run the tool:
1. Install ```service.pfx``` certificate from Certificates folder on the each server instance and on the client machine
2. Add installed certificate for the following IP ports on each server instance:
	* 8734
	* 8736
	* 9734
	* 9736
	* 7834
3. Install ```client.pfx``` certificate from Certificates folder on the client machine
4. Build and run WCF self-hosted service (```WcfServiceHost``` project) on the each server instance. Please make sure that each service instance has its own distinct value from 1 to N (N - amount of the server instances) of ServersNumber parameter, which is defined in the ```App.config```
5. Set up load balancing solution
6. Add the load balancer's IP address for ```testservice``` hostname on the client machine
7. Open ```WCFClient``` solution
7. Change ```testclient``` address from ```WsDualHttpEndPoint``` binding to the actual client's IP address
8. Set up test parameters in the ```App.config```
9. Choose and run the corresponding integration test from ```StatelessServiceTest``` or ```StatefulServiceTest``` files.

## Configurations for load balancers
This repository also contains configurations for appropriate scenarios for the popular load balancing solutions:
* Linux Virtual Server 1.2.1
* Microsoft Network Load Balancer 6.2
* HAProxy 1.6.3
* Nginx 1.9.15
* Varnish 4.1.1


