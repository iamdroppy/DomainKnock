```
 $   _ _
 $   | |__ _ _   ___  __ | |__ ___  _ _
 $   | / /| ' \ / _ \/ _|| / // -_)| '_|
 $   |_\_\|_||_|\___/\__||_\_\\___||_|
 $   seg-fault
```

# 🤷 What is this?

A bruteforce tool to help find a certain host within a range of IP addresses 

This is a simple project I did for research. I am not responsible for how it is used. 

It will check for port 80/443 for every IP given.

This is very helpful if the machine is using a reverse proxy like nginx, for example. A nginx reverse proxy set up will answer the HTTP request to many different domains, that a simple TCP scan using something like NMAP may not be enough! This tool helps you find which IP:port in a given endpoint responds to a given hostname (and how they respond).

# 🔨 Docker build/run

Building the image
```bash
git clone https://github.com/iamdroppy/DomainKnock.git domain-knock
cd domain-knock
docker build . --tag domain-knock/latest
```

Running:
```bash
docker run --rm -it domain-knock/latest --help
```

*❗Tip: pass **-e USE_CLI=1** to use a cli-only mode instead of the traditional argument-based mode.*

# 🔖 Usage

```
Tool:
  -v, --verbose           Verbosity Level (0 = Info, 1 = Debug, 2 = Trace)
  --help                  Display the help screen.
  --version               Display version information.

Bruteforce settings:
  -h, --hostname          Required. The hostname the IP(s) should respond to.

IP settings:
  --from                  Required. Start IP address. Use only this argument if you want to check a single IP Address
  --to                    End IP address. Use only --from if you want to check a single IP Address

Port settings (you can choose one or pick both):
  --http-ports            (Group: Ports) Http Ports (e.g. '80,8080-8099,9090-9100')
  --https-ports           (Group: Ports) Https (HTTP over SSL) Ports (e.g. '443,8443-8449')

Timespan settings:
  -t, --timeout           Timeout (in seconds) of each TCP connection and protocol negotiation (Default: 5)
  -d, --progress-delay    Delay (in seconds) between each progress scanning message. 0 to disable. (Default: 30)

Protocol settings:
  --agent                 User-Agent passed via Headers (Default: 'DomainKnock')
  --http2                 Attempts to write HTTP/2 instead of HTTP/1.1. THIS IS NOT THE SOLUTION FOR HTTP/2, rather a bypass for **some** reverse proxies.
  ```
*❗Tip 1: using --cli without any other arguments will give you the option to write commands inside the CLI, which is helpful if you are doing many small commands.*

*❗Tip 2: if you want to use a single IP instead of an IP-range, just use the --from argument, without the --to.*

## 🎀 Scan arguments sample

`-h google.com --from 10.0.0.0 --to 10.0.1.3 --http-ports 80,8080-8090 --https-ports 443,8443,9443 -d 5 -v 1`:
 - Scans from IP 10.0.0.0 until 10.0.1.3 *(IPs ending in .0 or .255 won't be scanned)*. For each IP, it will make, for each port, a HTTP connection. `80, 8080, 8081, ..., 8090`, and HTTP-over-SSL (https) ports `443, 8443, 9443`.
 - **-d** will set the progress-delay to 5, meaning every 5 seconds a log will be shown stating where you are in progress (i.e. how many hosts you've already scanned).
 - **-v** will set the verbosity level, which ranges from 0 to 2.

### 🧱 Port list format
The port format is as follows: 10-20,30,40-60

Results in the following: all ports from: 10 to 20, 30, and 40 to 60.
```
Input: 10-20,30,40-60

10 to 20 **(10 ports)** + 30 **(1 port)** + 40 to 60 **(20 ports)**
```
### 🧭 Verbosity levels

Each level inherits logs from the verbosity below.

 - Trace (*-v 2*): show-all mode. Logs everything, even when writing/reading from the TCP stream.
 - Debug (*-v 1*): non-blind mode. Logs when you've made any progress and shows semi-vital information such as when a TCP connection has been made (before the actual protocol/ssl negotiation).
 - Info (*-v 0 or default*): quiet mode. Logs only when you've made a successful connection and some simple information like the title of the page or the result.

### ⌨️ CLI mode

The CLI mode allows you to write the command-line arguments without having to restart the binary every time. When the scan finishes, it will prompt you again to type another command, and so on.

So instead of:
```
user@localhost> knocker --some --arguments
user@localhost> knocker --some --other --scan
```

The behaviour is the same with the CLI mode, but it will keep the application alive:
```
user@localhost> knocker --cli
> --some --arguments
> --some --other --scan
>
```

On the example above, it uses the argument --cli, but you can also define USE_CLI=1 or USE_CLI=true on your Environment Variables. Docker sample:
```bash
docker run --rm -it -e USE_CLI=1 domain-knock/latest
```

# 📗 Changelog

## v1.2.0.0
   - [x] Custom ports (using --http-ports and/or --https-ports - it will automatically add the SSL protocol over for ports declared as SSL) 
   - [x] Custom user-agent

# 📕 Planned/to-do list

   - [ ] Multitasking scan
   - [ ] Custom headers (currently, it is hardcoded with HTTP's most required headers by webservers)
   - [ ] Better output
   - [ ] HTTP/2 and HTTP/3 - currently --http2 attempts to write HTTP/2 instead of HTTP/1.1. WHICH IS NOT THE SOLUTION FOR HTTP/2, rather a bypass for **some** reverse proxies.
