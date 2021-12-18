```
_ _
| |__ _ _   ___  __ | |__ ___  _ _
| / /| ' \ / _ \/ _|| / // -_)| '_|   Hostname brute-force for IP ranges - v1.2.0.0
|_\_\|_||_|\___/\__||_\_\\___||_|

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
  -v, --verbose           Verbosity Level (0 = Info, 1 = Debug, 2 = Trace)

  -h, --hostname          Required. The hostname the IP(s) should respond to.

  --from                  Required. Start IP address. Use only this argument if you want to check a single IP Address

  --to                    End IP address. Use only --from if you want to check a single IP Address

  --http-ports            (Group: Ports) Http Ports (e.g. '80,8080-8099,9090-9100')

  --https-ports           (Group: Ports) Https (HTTP over SSL) Ports (e.g. '443,8443-8449')

  -t, --timeout           Timeout (in seconds) of each TCP connection and protocol negotiation (Default: 5)

  -d, --progress-delay    Delay (in seconds) between each progress scanning message. 0 to disable. (Default: 30)

  --agent                 User-Agent passed via Headers (Default: 'DomainKnock')

  --help                  Display this help screen.

  --version               Display version information.
  ```
*❗tip: using --cli without any other arguments will give you the option to write commands inside the CLI, which is helpful if you are doing many small commands.*

## 🎀 Sample:

`-h google.com --from 10.0.0.0 --to 10.0.1.3 --http-ports 80,8080-8090 --https-ports 443,8443,9443 -d 5 -v 1`:
 - Scans from IP 10.0.0.0 until 10.0.1.3 *(IPs ending in .0 or .255 won't be scanned)*. For each IP, it will make, for each port, a HTTP connection. `80, 8080, 8081, ..., 8090`, and HTTP-over-SSL (https) ports `443, 8443, 9443`.
 - It will set the progress-delay to 5, meaning every 5 seconds you will receive a message stating where you are in progress (i.e. how many hosts you've already scanned).
 - Finally, the **-v** will give you the verbosity level which ranges from 0 to 2.

### 🧱 Port list format:
The port format is as follows: 10-20,30,40-60
Results in the following: all ports from: 10 to 20, 30, and 40 to 60.

### 🧭 Verbosity levels

(each level includes everything from the level above, and so on).

 - Info (*-v 0 or default*): quiet mode. Logs only when you've made a successful connection and some simple information like the title of the page or the result.
 - Debug (*-v 1*): non-blind mode. Logs when you've made any progress and shows semi-vital information such as when a TCP connection has been made (before the actual protocol/ssl negotiation).
 - Trace (*-v 2*): show-all mode. Logs everything, even when writing/reading from the TCP stream.

# 📗 Changelog

## v1.2.0.0
   - [x] Custom ports (using --http-ports and/or --https-ports - it will automatically add the SSL protocol over for ports declared as SSL) 
   - [x] Custom user-agent

# 📕 Planned/to-do list

   - [ ] Multitasking scan
   - [ ] Custom headers (currently, it is hardcoded with HTTP's most required headers by webservers)
   - [ ] Better output
