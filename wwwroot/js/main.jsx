
if(typeof window !== 'undefined') {
  if (window.bowser.mobile) {
    document.documentElement.classList.add('mobile');
  } else if (window.bowser.tablet) {
    document.documentElement.classList.add('tablet');
  } else {
    document.documentElement.classList.add('desktop');
  }
}

class TestUrlForm extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      url: props.url || '',
      error: '',
      outputPlaceholder: 'The response will appear here',
      output: null,
      all: true,
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

    fetch(`/api/metadata?url=${this.urlInput.value.trim()}${this.state.all ? '&all=true' : ''}&priority=${this.state.priority}`)
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

  onAllChange = all => {
    this.setState({ all });
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
        <Options all={this.state.all} onAllChange={this.onAllChange}
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

  onAllChange = e => {
    this.props.onAllChange && this.props.onAllChange(e.target.checked);
  }

  onPriorityChange = e => {
    this.props.onPriorityChange && this.props.onPriorityChange(e.target.value);
  }

  render() {
    return (
      <div>
        <a className={`options-button${this.state.open ? ' open' : ''}`} onClick={this.toggle}>{this.state.open ? 'Hide' : 'Show'} options</a>
        <div className="options">
          <label className="checkbox">
            <span className="label-text">Get all</span>
            <input type={'checkbox'} checked={this.props.all} onChange={this.onAllChange}/>
            <span className="checkmark"></span>
          </label>
          <br/>
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
    );
  }
}