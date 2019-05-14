
if(typeof window !== 'undefined') {
  if (window.bowser.mobile) {
    document.documentElement.classList.add('mobile');
  } else if (window.bowser.tablet) {
    document.documentElement.classList.add('tablet');
  } else {
    document.documentElement.classList.add('desktop');
  }

  domready(() => {
    Array.from(document.querySelectorAll('.type-expand-button')).forEach(e => {
      e.addEventListener('click', () => {
        if (e.classList.contains('open')) {
          e.classList.remove('open');
        } else {
          e.classList.add('open');
        }
      })
    });
  });
}

class TestUrlForm extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      url: props.url || '',
      error: '',
      outputPlaceholder: 'The response will appear here',
      output: null,
      endpoint: 'Basic',
      priority: 'OpenGraph'
    }
  }

  onSubmit = e => {
    e.preventDefault();

    if (!this.state.url) {
      this.setState({ error: 'Please enter a url' });
      return;
    }

    if (this.urlInput.value.indexOf('://') === -1) {
      this.urlInput.value = `https://${this.urlInput.value}`;
    }

    if (!this.urlInput.checkValidity()) {
      this.setState({ error: 'Please enter a valid url' });
      return;
    }

    this.setState({ error: '', output: null, outputPlaceholder: '' });

    fetch(`/api/metadata${this.state.endpoint !== 'Basic' ? `/${this.state.endpoint.toLowerCase()}` : ''}?url=${this.urlInput.value.trim()}&priority=${this.state.priority}`)
      .then(response => response.json())
      .then(json => {
        const codeHtml = Prism.highlight(JSON.stringify(json, null, ' '), Prism.languages.json, 'json');
        this.setState({ output: codeHtml, outputPlaceholder: '' });
      })
      .catch(() => this.setState({
        output: null,
        outputPlaceholder: 'An error occured whilst trying to retrieve the metadata'
      }));
  }

  onUrlChange = e => {
    this.setState({ url: e.target.value });
  }

  onEndpointChange = endpoint => {
    this.setState({ endpoint });
  }

  onPriorityChange = priority => {
    this.setState({ priority });
  }

  render() {
    return (
      <form id="test-url-form" noValidate onSubmit={this.onSubmit}>
        <label htmlFor="url-input">Enter a Url:</label>
        <br />
        <div className='input-container'>
          <input ref={i => { this.urlInput = i }} required type="url" value={this.state.url} id="url-input" name="url" onChange={this.onUrlChange} />
          <button type="submit" className="button">Submit</button>
        </div>
        <span className='error'>{this.state.error}</span>
        <Options endpoint={this.state.endpoint} onEndpointChange={this.onEndpointChange}
          priority={this.state.priority} onPriorityChange={this.onPriorityChange} />
        <pre>
          {this.state.output
            ? (
              <code id="output" dangerouslySetInnerHTML={{ __html: this.state.output }}>
              </code>
            )
            : (
              <code id="output">
                {this.state.outputPlaceholder}
              </code>
            )}
        </pre>
      </form>
    );
  }
}

class Options extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      open: false
    }
  }

  toggle = () => {
    this.setState({ open: !this.state.open });
  }

  onEndpointChange = e => {
    this.props.onEndpointChange && this.props.onEndpointChange(e.target.value);
  }

  onPriorityChange = e => {
    this.props.onPriorityChange && this.props.onPriorityChange(e.target.value);
  }

  render() {
    return (
      <div>
        <a className={`options-button${this.state.open ? ' open' : ''}`} onClick={this.toggle}>{this.state.open ? 'Hide' : 'Show'} options</a>
        <div className="options">
          Type:
          {['Basic', 'All', 'Tree'].map(type => (
            <label key={type} className="radio">
              <span>{type}</span>
              <input type={'radio'} value={type} checked={this.props.endpoint === type} onChange={this.onEndpointChange} />
              <span className="mark"></span>
            </label>
          ))}
          <br />
          <div className={this.props.endpoint !== "Basic" ? 'disabled' : null}>
            Priority:
            {['OpenGraph', 'Twitter', 'Generic'].map(type => (
              <label key={type} className="radio">
                <span>{type}</span>
                <input type={'radio'} value={type} checked={this.props.priority === type} onChange={this.onPriorityChange} />
                <span className="mark"></span>
              </label>
              ))}
          </div>
        </div>
      </div>
    );
  }
}